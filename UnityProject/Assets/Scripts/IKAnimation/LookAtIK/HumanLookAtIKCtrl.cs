using DG.Tweening;
using UnityEngine;

namespace IKAnimation
{
    public class HumanLookAtIKCtrl : LookAtIKCtrl
    {
        private void Update()
        {
            if (this.IKActive)
            {
                var tempFov = this.FovProbe();
                if (tempFov != this.InView)
                {
                    this.InView = tempFov;
                    if (this.InView)
                        this.FadeIn();
                    else
                        this.FadeOut();
                }
                this.InView = tempFov;
            }
        }
        
        /// <summary>
        /// 视野检测
        /// </summary>
        /// <returns></returns>
        public override bool FovProbe()
        {
            if (this.TargetTrans == null)
                return false;

            if (this.Is3DView)
                return LookAtProbe.CylinderProbe(this.transform, this.TargetTrans, this.IKConfig);
            else
                return LookAtProbe.SectorProbe(this.transform, this.TargetTrans, this.IKConfig);
        }
    }
}