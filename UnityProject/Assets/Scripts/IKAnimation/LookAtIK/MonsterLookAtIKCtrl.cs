﻿using DG.Tweening;
using UnityEngine;

namespace IKAnimation
{
    public class MonsterLookAtIKCtrl : LookAtIKCtrl
    {
        /// <summary>
        /// 扇形视野检测
        /// </summary>
        /// <returns></returns>
        public override bool FovProbe(Transform rTarget = null)
        {
            if (this.CurTargetTrans == null)
                return false;
            if (this.Is3DView)
                return LookAtProbe.CylinderProbe(this.BodyTrans, rTarget != null ? rTarget : this.LookAtPoint.transform, this.IKConfig);
            else
                return LookAtProbe.SectorProbe(this.BodyTrans, rTarget != null ? rTarget : this.LookAtPoint.transform, this.IKConfig);
        }
    }
}


