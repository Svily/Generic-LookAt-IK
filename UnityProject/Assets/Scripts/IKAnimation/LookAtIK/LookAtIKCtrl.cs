using System;
using DG.Tweening;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IKAnimation
{
    [RequireComponent(typeof(LookAtIK)), DisallowMultipleComponent]
    public class LookAtIKCtrl : IKBase
    {
        public LookAtIK             LookAtIK;
        
        public LookAtIKConfig       IKConfig;
        
        [LabelText("当前目标"), HideInInspector]
        public Transform            CurTargetTrans;

        [LabelText("新目标")]
        public Transform            NewTargetTrans;
        
        [LabelText("身体")]
        public Transform            BodyTrans;
        
        public bool                 Is3DView;
        
        public bool                 InView;
        
        public GameObject           LookAtPoint;
        
        public IKSolverLookAt       IKSover             => this.LookAtIK?.solver;

        private Tweener             SwitchTweener;
        
        private Tweener             IKTweener;
        
        /// <summary>
        /// 探测方法，需根据策划需求重写
        /// </summary>
        /// <returns></returns>
        public virtual bool FovProbe(Transform rTarget = null) { return false;}
        
        
        private void Awake()
        {
            this.IKActive = false;
            this.InView = false;
            //初始化时把全局权重设置为0
            this.IKSover.IKPositionWeight = 0;
        }
        
        private void Update()
        {
            if (!this.IKActive)
                return;
            // 同一目标则检测范围
            if (this.CurTargetTrans != null && this.NewTargetTrans == this.CurTargetTrans)
                this.UpdateView();
            else //切换目标
                this.UpdateTarget();
        }
        
        /// <summary>
        /// IK初始化
        /// </summary>
        public void InitLookAtIK(Transform tBaseTrans, GameObject gLookAtPoint)
        {
            this.LookAtPoint = gLookAtPoint;
            this.BodyTrans = tBaseTrans;
            if (this.LookAtIK == null || this.IKConfig == null)
            {
                this.IKActive = false;
                Debug.LogError("LookAt IK or IKConfig is Null");
                return;
            }
            //绑定LookAtPoint
            this.IKSover.target = this.LookAtPoint.transform;
            
            //设置权重
            this.UpateIKConfig();
        }
        
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
        
        private void UpdateTarget()
        {
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
                }
                else //切换注视目标
                {
                    // 大于配置角度，需要每帧修正坐标
                    if (targetAngle > this.IKConfig.TargetSwithAngle)
                    {
                        this.SwitchTweener?.Kill();
                        this.LookAtPoint.transform.SetParent(this.NewTargetTrans);
                        
                        //判断新旧目标是否位于同侧
                        var curIsRight = Vector3.Dot(this.transform.right, curTargetVec) > 0 ;
                        var newIsRight = Vector3.Dot(this.transform.right, newTargetVec) > 0 ;
                        
                        //回正后再转向
                        if (this.IKConfig.ST4Forward && (curIsRight != newIsRight))
                        {
                            this.CurTargetTrans = this.NewTargetTrans;
                            this.SwitchTweener?.Kill();
                            //回正
                            this.SwitchTweener =  DOTween.To(() => this.IKSover.IKPositionWeight, 
                                (x) => this.IKSover.IKPositionWeight = x, 
                                0,
                                this.IKConfig.SwitchFadeOutTime).SetEase(this.IKConfig.ResetCurveType);
                            
                            //回正完成后设置新目标点
                            this.SwitchTweener.onComplete = () =>
                            {
                                this.SetLookAtPoint();
                                this.FadeIn(fTime);
                            };
                        }
                        else // 均速转向过去
                        {
                            this.CurTargetTrans = this.NewTargetTrans;
                            this.SwitchTweener?.Kill();
                            this.SwitchTweener = DOTween.To(() => this.LookAtPoint.transform.localPosition, 
                                (x) => { this.LookAtPoint.transform.localPosition = x; },
                                new Vector3(0, 0, 0), 
                                fTime).SetEase(this.IKConfig.LookCurveType);
                            this.SwitchTweener.onComplete = this.SetLookAtPoint;
                        }
                    }
                    else //直接转过去，一般不会走到这里
                    {
                        this.CurTargetTrans = this.NewTargetTrans;
                        this.SetLookAtPoint();
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
                this.SwitchTweener.onComplete = this.SetLookAtPoint;
                this.FadeOut(this.IKConfig.FadeOutTime);
            }
        }
        
        

        public void UpateIKConfig()
        {
            this.LookAtIK.solver.SetLookAtWeight(this.IKConfig.Weight, 
                this.IKConfig.BodyWeight, 
                this.IKConfig.HeadWeight, 
                this.IKConfig.EyesWeight, 
                this.IKConfig.ClampWeight, 
                this.IKConfig.ClampHeadWeight, 
                this.IKConfig.ClampEyesWeight);
        }
        
        public override void OpenIK()
        {
            this.FadeIn(this.IKConfig.FadeInTime);
        }
        
        public override void CloseIK()
        {
            this.FadeOut(this.IKConfig.FadeOutTime);
        }
        
        public void FadeIn(float fTime, Action rCall = null)
        {
            this.IKTweener?.Kill();
            this.IKTweener = DOTween.To(() => this.IKSover.IKPositionWeight, 
                (x) =>  this.IKSover.IKPositionWeight = x, 
                this.IKConfig.Weight,
                fTime).SetEase(this.IKConfig.LookCurveType);
            this.IKTweener.onComplete = () =>
            {
                rCall?.Invoke();
            };
        }

        public void FadeOut(float fTime, Action rCall = null)
        {
            this.IKTweener?.Kill();
            this.IKTweener =  DOTween.To(() => this.IKSover.IKPositionWeight, 
                (x) => this.IKSover.IKPositionWeight = x, 
                0,
                fTime).SetEase(this.IKConfig.ResetCurveType);
            this.IKTweener.onComplete = () =>
            {
                rCall?.Invoke();
            };
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
            if (this.LookAtIK == null)
            {
                Debug.LogError("Look IK Component is Null !!!");
                return;
            }

            this.IKActive = true;
            this.NewTargetTrans = rTarget;
            if (this.CurTargetTrans == null)
            {
                this.CurTargetTrans = this.NewTargetTrans;
                this.SetLookAtPoint();
            }
        }

        private void SetLookAtPoint()
        {
            this.LookAtPoint.transform.SetParent(this.CurTargetTrans);
            this.LookAtPoint.transform.localPosition = Vector3.zero;
        }

        private void SetLookAtPointParent()
        {
            this.LookAtPoint.transform.SetParent(this.CurTargetTrans);
        }

        private void LookAtNewTarget(Transform rTarget, float fTime)
        {
            this.CurTargetTrans = rTarget;
            this.SetLookAtPoint();
            this.FadeIn(fTime);
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