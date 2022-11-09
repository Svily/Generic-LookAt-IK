using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace IKAnimation
{
    [DisallowMultipleComponent]
    public class LookAtIKCtrl : IKBase
    {
        [LabelText("IK组件列表")]
        public List<LookAtIK>       LookAtIKList;

        [LabelText("IKParam 列表")] 
        public List<LookAtParams>   LookAtParamsList;

        [LabelText("IK配置")]
        public LookAtIKConfig       IKConfig;
        
        [LabelText("当前目标"), HideInInspector]
        public Transform            CurTargetTrans;

        [LabelText("新目标"),HideInInspector]
        public Transform            NewTargetTrans;
        
        [LabelText("身体"), HideInInspector]
        public Transform            BodyTrans;
        
        [LabelText("是否为3D视野(默认false)")]
        public bool                 Is3DView;
        
        [LabelText("是否处于视野内")]
        public bool                 InView;
        
        [LabelText("注视点")]
        public GameObject           LookAtPoint;
        
        
        private List<Tweener>       IKTweenerList       = new List<Tweener>();
        private Tweener             SwitchTweener;
        private bool                IsIdle;
        private bool                IsSwitching;

        /// <summary>
        /// 探测方法，需根据策划需求重写
        /// </summary>
        /// <returns></returns>
        public virtual bool FovProbe(Transform rTarget = null)
        {
            return LookAtProbe.CylinderProbe(this.BodyTrans, rTarget != null ? rTarget :this.LookAtPoint.transform, this.IKConfig);
        }

        #region MonoMethod
        
        private void Awake()
        {
            this.IsIdle = true;
            this.IKActive = true;
            this.InView = false;
            this.BodyTrans = this.transform;
            if (this.LookAtPoint == null)
            {
                this.LookAtPoint = new GameObject("LookAtPoint");
                this.LookAtPoint.transform.SetParent(this.transform);
                this.LookAtPoint.transform.localPosition = Vector3.zero;
            }
            this.InitIKSolver();
            //初始化时把全局权重设置为0
            this.SetIKSolver(0);

#if UNITY_EDITOR
            LooKAtIKManager.instance?.AddIKCtrl("Test", this);
#endif
        }
        
        private void Update()
        {
            if (!this.IKActive)
                return;

            if (this.IsIdle)
            {
                //走Idle逻辑,idle时注视视野范围内最近的物体
            }

            // 同一目标则检测范围
            if (!this.IsSwitching && this.CurTargetTrans != null)
                this.UpdateView();
            
        }

        private void OnDestroy()
        {
            Destroy(this.LookAtPoint);
#if UNITY_EDITOR
            LooKAtIKManager.instance?.RemoveIKCtrl("Test");
#endif
        }

        #endregion
        
        
        private void UpdateView()
        {
            var tempView = this.GetLookProbe(null);
            if (tempView != this.InView)
            {
                this.InView = tempView;
                if (this.InView)
                    this.FadeIn(this.IKConfig.FadeInTime);
                else
                    this.FadeOut(this.IKConfig.FadeOutTime);
            }
        }
        
        private void SwitchTarget()
        {
            this.IsSwitching = true;
            // 空目标表示不注视
            if (this.NewTargetTrans == null)
            {
                this.SetLookAtNull();
                return;
            }
            
            //先判断新目标是否在视野内
            var bInView = this.GetLookProbe(this.NewTargetTrans);
            if (bInView)
            {
                var curTargetVec = this.CurTargetTrans == null ? this.BodyTrans.forward : this.CurTargetTrans.position - this.transform.position;
                var newTargetVec = this.NewTargetTrans.position - this.transform.position;
                var targetAngle = Vector3.Angle(curTargetVec, newTargetVec);
                var fTime = this.GetIKConfigTime(targetAngle);
                // 当前锁定的目标不在视野范围内，直接看向新目标
                if (this.CurTargetTrans == null || !this.GetLookProbe(null))
                {
                    this.LookAtNewTarget(this.NewTargetTrans, targetAngle);
                    this.IsSwitching = false;
                }
                else //切换注视目标
                {
                    this.CurTargetTrans = this.NewTargetTrans;
                    // 大于配置角度，需要修正LookAtPoint坐标
                    if (targetAngle > this.IKConfig.TargetSwithAngle)
                    {
                        this.CurTargetTrans = this.NewTargetTrans;
                        this.SwitchTweener?.Kill();
                        this.LookAtPoint.transform.SetParent(this.NewTargetTrans);
                        
                        //判断新旧目标是否位于同侧
                        var curIsRight = Vector3.Dot(this.transform.right, curTargetVec) > 0 ;
                        var newIsRight = Vector3.Dot(this.transform.right, newTargetVec) > 0 ;

                        //回正后再转向
                        if (this.IKConfig.ST4Forward && (curIsRight != newIsRight))
                        {
                            this.SwitchTweener?.Kill();
                            //回正
                            this.FadeOut(this.IKConfig.SwitchFadeOutTime);
                            //回正完成后设置新目标点
                            targetAngle = Vector3.Angle(this.BodyTrans.forward, newTargetVec);
                            fTime = this.GetIKConfigTime(targetAngle);
                            this.SwitchTweener.onComplete = () =>
                            {
                                this.ResetLookAtPoint();
                                this.FadeIn(fTime);
                                this.IsSwitching = false;
                            };
                        }
                        else // 均速转向过去
                        {
                            this.SwitchTweener?.Kill();
                            this.SwitchTweener = DOTween.To(() => this.LookAtPoint.transform.localPosition, 
                                (x) => { this.LookAtPoint.transform.localPosition = x; },
                                new Vector3(0, 0, 0), 
                                fTime).SetEase(this.IKConfig.LookCurveType);
                            this.SwitchTweener.onComplete = () =>
                            {
                                this.ResetLookAtPoint();
                                this.IsSwitching = false;
                            };
                        }
                    }
                    else //直接转过去，一般不会走到这里
                    {
                        this.ResetLookAtPoint();
                        this.IsSwitching = false;
                    }
                }
            }else
            {
                //不在范围内就回正
                this.CurTargetTrans = this.NewTargetTrans;
                this.SetLookAtPointParent();
                this.SwitchTweener?.Kill();
                this.SwitchTweener = DOTween.To(() => this.LookAtPoint.transform.localPosition, 
                    (x) => { this.LookAtPoint.transform.localPosition = x; },
                    new Vector3(0, 0, 0), 
                    0.5f).SetEase(this.IKConfig.ResetCurveType);
                this.SwitchTweener.onComplete = () =>
                {
                    this.ResetLookAtPoint();
                    this.IsSwitching = false;
                };
                this.FadeOut(this.IKConfig.FadeOutTime);
            }
        }

        public void InitIKSolver()
        {
            if (this.LookAtParamsList.Count == this.LookAtIKList.Count)
            {
                for (int i = 0; i < this.LookAtIKList.Count; i++)
                {
                    var x = this.LookAtIKList[i];
                    var y = this.LookAtParamsList[i];
                    x.solver.target = this.LookAtPoint.transform;
                    x.solver.SetLookAtWeight(y.Weight, 
                        y.BodyWeight, 
                        y.HeadWeight, 
                        y.EyesWeight, 
                        y.ClampWeight, 
                        y.ClampHeadWeight, 
                        y.ClampEyesWeight);
                    this.FixAxisByParam(x, y);
                }
                
            }
            else //使用全局配置
            {
                this.LookAtIKList?.ForEach((x) =>
                {
                    x.solver.target = this.LookAtPoint.transform;
                    x.solver.SetLookAtWeight(this.IKConfig.Weight, 
                        this.IKConfig.BodyWeight, 
                        this.IKConfig.HeadWeight, 
                        this.IKConfig.EyesWeight, 
                        this.IKConfig.ClampWeight, 
                        this.IKConfig.ClampHeadWeight, 
                        this.IKConfig.ClampEyesWeight);
                    this.FixAxis(x);
                });
            }
            
            
        }

        private void  FixAxis(LookAtIK x)
        {
            //修正Head Axis
            if (this.IKConfig.FixHeadAxis)
            {
                Vector3 axis;
                if (this.IKConfig.CustomAxis)
                {
                    axis = this.IKConfig.AxisVec;
                }
                else
                {
                    axis = this.IKConfig.AxisType switch
                    {
                        AxisType.Up => x.solver.head.transform.up,
                        AxisType.Down => -x.solver.head.transform.up,
                        AxisType.Left => -x.solver.head.transform.right,
                        AxisType.Right => x.solver.head.transform.right,
                        _ => -x.solver.head.transform.right
                    };
                }
                    
                x.solver.head.FixAxis(this.IKConfig.OffsetAngle, axis);
                x.solver.spine.ForEach(bone =>
                {
                    bone.FixAxis(this.IKConfig.OffsetAngle, axis);
                });
            }
        }
        
        private void  FixAxisByParam(LookAtIK x, LookAtParams y)
        {
            //修正Head Axis
            if (y.FixHeadAxis)
            {
                Vector3 axis;
                if (y.CustomAxis)
                {
                    axis = y.AxisVec;
                }
                else
                {
                    axis = y.AxisType switch
                    {
                        AxisType.Up => x.solver.head.transform.up,
                        AxisType.Down => -x.solver.head.transform.up,
                        AxisType.Left => -x.solver.head.transform.right,
                        AxisType.Right => x.solver.head.transform.right,
                        _ => -x.solver.head.transform.right
                    };
                }
                    
                x.solver.head.FixAxis(y.OffsetAngle, axis);
                x.solver.spine.ForEach(bone =>
                {
                    bone.FixAxis(y.OffsetAngle, axis);
                });
            }
        }

        private void SetIKSolver(float fValue)
        {
            this.LookAtIKList?.ForEach((x) =>
            {
                x.solver.SetLookAtWeight(fValue);
            });
        }
        
        public override void OpenIK()
        {
            // 在注视范围内才开启，避免硬切
            if (this.InView)
                this.FadeIn(this.IKConfig.FadeInTime);
        }
        
        public override void CloseIK()
        {
            this.FadeOut(this.IKConfig.FadeOutTime);
        }
        
        public void FadeIn(float fTime, Action rCall = null)
        {
            this.IKTweenerList.ForEach(x=>x.Kill());
            this.IKTweenerList.Clear();
            for (int i = 0; i < this.LookAtIKList.Count; i++)
            {
                var nIndex = i;
                var tempTweener =  DOTween.To(() =>this.LookAtIKList[nIndex].solver.IKPositionWeight, 
                    (x) =>  this.LookAtIKList[nIndex].solver.IKPositionWeight = x, 
                    this.IKConfig.Weight,
                    fTime).SetEase(this.IKConfig.LookCurveType);
                tempTweener.onComplete = () =>
                {
                    rCall?.Invoke();
                };
                
                this.IKTweenerList.Add(tempTweener);
            }
        }

        public void FadeOut(float fTime, Action rCall = null)
        {
            this.IKTweenerList.ForEach(x=>x.Kill());
            this.IKTweenerList.Clear();
            for (int i = 0; i < this.LookAtIKList.Count; i++)
            {
                var nIndex = i;
                var tempTweener =  DOTween.To(() => this.LookAtIKList[nIndex].solver.IKPositionWeight, 
                    (x) => this.LookAtIKList[nIndex].solver.IKPositionWeight = x, 
                    0,
                    fTime).SetEase(this.IKConfig.ResetCurveType);
                tempTweener.onComplete = () =>
                {
                    rCall?.Invoke();
                };
                
                this.IKTweenerList.Add(tempTweener);
            }
        }

        /// <summary>
        /// 取消注视
        /// </summary>
        public void SetLookAtNull()
        {
            this.CloseIK();
            this.InView = false;
            this.CurTargetTrans = null;
            this.NewTargetTrans = null;
            this.IsIdle = true;
            this.IsSwitching = false;
        }

        /// <summary>
        /// 设置注视目标
        /// </summary>
        /// <param name="rGo"></param>
        public void SetLookAtTarget(GameObject rGo)
        {
            if (rGo == null)
                this.SetLookAtNull();
            this.SetLookAtTarget(rGo.transform);
        }
        
        /// <summary>
        /// 设置注视目标
        /// </summary>
        /// <param name="rTarget"></param>
        public void SetLookAtTarget(Transform rTarget)
        {
            if (this.LookAtIKList == null || this.LookAtIKList.Count <= 0)
            {
                Debug.LogError("Look IK Component is Null !!!");
                return;
            }

            this.IKActive = true;
            this.IsIdle = false;
            this.NewTargetTrans = rTarget;
            if (this.CurTargetTrans == null)
            {
                this.CurTargetTrans = this.NewTargetTrans;
                this.ResetLookAtPoint();
            }
            else
            {
                this.SwitchTarget();
            }
        }

        /// <summary>
        /// 注视某个点
        /// </summary>
        /// <param name="rTargetPoint"></param>
        /// <param name="fTime"></param>
        public void SetLookAtPoint(Vector3 rTargetPoint, float fTime = 0f)
        {
            var rNewTargetVec = rTargetPoint - this.BodyTrans.position;
            var rCurTargetVec = this.LookAtPoint.transform.position - this.BodyTrans.position;
            if (fTime == 0)
                fTime = this.GetIKConfigTime(Vector3.Angle(rCurTargetVec, rNewTargetVec));
            this.SwitchTweener?.Kill();
            this.SwitchTweener = DOTween.To(() => this.LookAtPoint.transform.position,
                (x) => this.LookAtPoint.transform.position = x,
                rTargetPoint, fTime).SetEase(this.IKConfig.LookCurveType);
        }

        private void ResetLookAtPoint()
        {
            this.LookAtPoint.transform.SetParent(this.CurTargetTrans);
            this.LookAtPoint.transform.localPosition = Vector3.zero;
        }

        private void SetLookAtPointParent()
        {
            this.LookAtPoint.transform.SetParent(this.CurTargetTrans);
        }

        private void LookAtNewTarget(Transform rTarget, float fTime, Action rCall = null)
        {
            this.CurTargetTrans = rTarget;
            this.ResetLookAtPoint();
            this.FadeIn(fTime, rCall);
        }
        
        private bool GetLookProbe(Transform rTarget)
        {
            switch (this)
            {
                case MonsterLookAtIKCtrl mCtrl:
                    return mCtrl.FovProbe(rTarget);
                case HumanLookAtIKCtrl hCtrl:
                    return hCtrl.FovProbe(rTarget);
                default:
                    return this.FovProbe();
            }
        }

        private float GetIKConfigTime(float fAngle)
        {

            float fTime = this.IKConfig.FadeInTime;
            var rList = this.IKConfig.DirAngelTimeList;
            if (rList == null || rList.Count <= 0)
            {
                return fTime;
            }
            for (int i = rList.Count - 1; i >= 0; i--)
            {
                var rTimeConfig = rList[i];
                if (fAngle > rTimeConfig.AngleValue)
                {
                    fTime = rTimeConfig.Time;
                    break;
                }
            }
            return fTime;
        }
    }
}