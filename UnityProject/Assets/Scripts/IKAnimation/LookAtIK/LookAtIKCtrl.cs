using DG.Tweening;
using RootMotion.FinalIK;
using UnityEngine;

namespace IKAnimation
{
    [RequireComponent(typeof(LookAtIK)), DisallowMultipleComponent]
    public class LookAtIKCtrl : IKBase
    {
        public LookAtIK             LookAtIK;
        
        public LookAtIKConfig       IKConfig;
        
        public Transform            TargetTrans; 
        
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
        public virtual bool FovProbe() { return false;}
        
        private void Awake()
        {
            this.IKActive = false;
            if (this.LookAtPoint == null)
            {
                this.LookAtPoint = new GameObject("LookAtPoint");
                this.LookAtPoint.transform.SetParent(this.transform);
            }
            this.InitLookAtIK();
            //初始化时把全局权重设置为0
            this.IKSover.IKPositionWeight = 0;
        }
        
        /// <summary>
        /// IK初始化
        /// </summary>
        private void InitLookAtIK()
        {
            if (this.LookAtIK == null || this.IKConfig == null)
            {
                this.IKActive = false;
                Debug.LogError("LookAt IK or IKConfig is Null");
                return;
            }
            //绑定LookAtPoint
            this.IKSover.target = this.LookAtPoint.transform;
            LooKAtIKManager.Instance.AddIKCtrl(this.transform.name, this);
            //设置权重
            this.UpateIKConfig();
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
            this.IKActive = true;
            this.FadeIn();
        }
        
        public override void CloseIK()
        {
            this.IKActive = false;
            this.FadeOut();
        }
        
        public void FadeIn()
        {
            this.IKTweener?.Kill();
            this.IKTweener = DOTween.To(() => this.IKSover.IKPositionWeight, 
                (x) =>  this.IKSover.IKPositionWeight = x, 
                this.IKConfig.Weight,
                this.IKConfig.FadeInTime);
        }

        public void FadeOut()
        {
            this.IKTweener?.Kill();
            this.IKTweener =  DOTween.To(() => this.IKSover.IKPositionWeight, 
                (x) => this.IKSover.IKPositionWeight = x, 
                0,
                this.IKConfig.FadeOutTime);
        }

        public void SetLookAtNull()
        {
            this.CloseIK();
            this.InView = false;
            this.TargetTrans = null;
        }

        
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
            
            // 空目标表示不注视，或者可以直接调用CloseIK
            if (rTarget == null)
            {
                this.SetLookAtNull();
                return;
            }
            
            // 锁一个初始目标
            if (this.TargetTrans == null)
            {
                this.IKActive = true;
                this.TargetTrans = rTarget;
                this.SetLookAtPoint();
                this.FadeIn();
            }
            else //切换注视目标
            {
                var curTargetVec = this.TargetTrans.position - this.transform.position;
                var newTargetVec = rTarget.position - this.transform.position;
                var targetAngle = Vector3.Angle(curTargetVec, newTargetVec);
                if (targetAngle > this.IKConfig.TargetSwithAngle)
                {
                    this.SwitchTweener?.Kill();
                    this.LookAtPoint.transform.SetParent(rTarget);
                    
                    //判断新旧目标是否位于同侧
                    var curIsRight = Vector3.Dot(this.transform.right, curTargetVec) > 0 ;
                    var newIsRight = Vector3.Dot(this.transform.right, newTargetVec) > 0 ;
                    
                    //回正后再转向
                    if (this.IKConfig.ST4Forward && (curIsRight == newIsRight))
                    {
                        this.SwitchTweener?.Kill();
                        
                        //回正
                        this.SwitchTweener =  DOTween.To(() => this.IKSover.IKPositionWeight, 
                            (x) => this.IKSover.IKPositionWeight = x, 
                            0,
                            this.IKConfig.FadeOutTime);
                        
                        //回正完成后设置新目标点
                        this.SwitchTweener.onComplete = () =>
                        {
                            this.TargetTrans = rTarget;
                            this.SetLookAtPoint();
                            this.IKActive = true;
                        };
                    }
                    else
                    {
                        this.IKActive = true;
                        this.SwitchTweener = DOTween.To(() => this.LookAtPoint.transform.localPosition, 
                            (x) => { this.LookAtPoint.transform.localPosition = x; },
                            new Vector3(0, 0, 0), 
                            0.5f);
                    }
                }
                else
                {
                    this.IKActive = true;
                    this.SetLookAtPoint();
                }
                
                this.TargetTrans = rTarget;
            }
        }

        private void SetLookAtPoint()
        {
            this.LookAtPoint.transform.SetParent(this.TargetTrans);
            this.LookAtPoint.transform.localPosition = Vector3.zero;
        }
        
        private void OnDestroy()
        {
            LooKAtIKManager.Instance.RemoveIKCtrl(this.transform.name);
            GameObject.Destroy(this.LookAtPoint);
        }
    }
}