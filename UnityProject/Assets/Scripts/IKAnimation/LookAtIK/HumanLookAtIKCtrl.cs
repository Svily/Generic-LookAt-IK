using DG.Tweening;
using UnityEngine;

namespace IKAnimation
{
    public class HumanLookAtIKCtrl : LookAtIKCtrl
    {
        
        /// <summary>
        /// 视野检测
        /// </summary>
        /// <returns></returns>
        public override bool FovProbe(Transform rTarget = null)
        {
            if (this.CurTargetTrans == null)
                return false;
            
            if (this.Is3DView)
                return LookAtProbe.CylinderProbe(this.BodyTrans, rTarget != null ? rTarget :this.LookAtPoint.transform, this.IKConfig, this.InView);
            else
                return LookAtProbe.SectorProbe(this.BodyTrans, rTarget != null ? rTarget :this.LookAtPoint.transform, this.IKConfig, this.InView);
        }
    }
}