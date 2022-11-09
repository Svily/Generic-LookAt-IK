using UnityEngine;

namespace IKAnimation
{
    public static class LookAtProbe
    {
        /// <summary>
        /// 扇形范围检测
        /// </summary>
        /// <param name="tBaseTrans"></param>
        /// <param name="tTargetTrans"></param>
        /// <param name="rIKConfig"></param>
        /// <param name="bInView"></param>
        /// <returns></returns>
        public static bool SectorProbe(Transform tBaseTrans, Transform tTargetTrans, LookAtIKConfig rIKConfig, bool bInView = false)
        {
            float configAngle = bInView ? rIKConfig.Angle : rIKConfig.DetectAngle;
            
            //XZ平面的视野范围计算
            //所有计算均在世界坐标系下进行
            //构建目标与观测者的向量
            Vector2 aimTargetHorVec = new Vector2(tTargetTrans.position.x, tTargetTrans.position.z);
            Vector2 forwardVec = new Vector2(tBaseTrans.forward.x, tBaseTrans.forward.z);
            Vector2 TargetHorVector = aimTargetHorVec - new Vector2(tBaseTrans.position.x, tBaseTrans.position.z);
            
            // 先判断是否在视距之内
            var distance = TargetHorVector.magnitude;
            if (distance > rIKConfig.ViewDistance)
                return false;
            
            // 判断是否在视角内
            var targetAngle = Vector2.Angle(forwardVec, TargetHorVector);
            return !(targetAngle > (configAngle / 2));
        }

        /// <summary>
        /// 圆锥形范围检测
        /// </summary>
        /// <param name="tBaseTrans"></param>
        /// <param name="tTargetTrans"></param>
        /// <param name="rIKConfig"></param>
        /// <param name="bInView"></param>
        /// <returns></returns>
        public static bool CylinderProbe(Transform tBaseTrans, Transform tTargetTrans, LookAtIKConfig rIKConfig, bool bInView = false)
        {
            float configAngle = bInView ? rIKConfig.Angle : rIKConfig.DetectAngle;
            
            //所有计算均在3D世界坐标系下进行
            //构建目标与观测者的向量
            Vector3 forwardVec = tBaseTrans.forward;
            Vector3 TargetVector = tTargetTrans.position - tBaseTrans.position;
            
            // 先判断是否在视距之内
            var distance = TargetVector.magnitude;
            if (distance > rIKConfig.ViewDistance)
                return false;
            
            // 判断是否在视角内
            var targetAngle = Vector3.Angle(forwardVec, TargetVector);
            return !(targetAngle > (configAngle / 2));
        }
    }
}