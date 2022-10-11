using Sirenix.OdinInspector;
using UnityEngine;

namespace IKAnimation
{
    [CreateAssetMenu(fileName = "New LookAtIK Config", menuName = "Final IK/Create LookAt Config")]
    public class LookAtIKConfig : SerializedScriptableObject
    {
        [LabelText("视角范围"), Range(0, 180)]
        public float        Angle;
        
        [LabelText("视野距离")]
        public float        ViewDistance        = 1000;

        [LabelText("回正速率曲线")]
        public AnimationCurve Curve;
        
        [LabelText("切换锁定目标回正?")]
        public bool         ST4Forward;
        
        [LabelText("切换目标回正时间")] 
        [ShowIf("@ST4Forward == true")]
        public float        SwitchFadeOutTime= 0.5f;
        
        [LabelText("直接转向所需时间")]
        [ShowIf("@ST4Forward == false")]
        public float        DireTurnToTime   = 1f;
        
        [LabelText("切换目标所需最小角度")]
        public float        TargetSwithAngle = 5f;

        [LabelText("目标进入视野锁定时间")] 
        public float        FadeInTime       = 0.5f;
        
        [LabelText("目标丢失回正时间")] 
        public float        FadeOutTime      = 0.5f;
        
        [LabelText("全局权重"), Range(0, 1)] 
        public float        Weight           = 1;
        
        [LabelText("身体权重"), Range(0, 1)] 
        public float        BodyWeight       = 0;
        
        [LabelText("头权重"), Range(0, 1)] 
        public float        HeadWeight       = 1f;
        
        [LabelText("眼睛权重"), Range(0, 1)] 
        public float        EyesWeight       = 0;
        
        [Range(0, 1), HideInInspector] 
        public float        ClampWeight      = 0.5f;
        
        [Range(0, 1), HideInInspector] 
        public float        ClampBodyWeight  = 0.5f;
        
        [Range(0, 1), HideInInspector] 
        public float        ClampHeadWeight  = 0.5f;
        
        [Range(0, 1), HideInInspector] 
        public float        ClampEyesWeight  = 0.5f;

#if UNITY_EDITOR
        [Button("更新权重值")]
        private void UpdateIKConfig()
        {
            foreach (var kv in LooKAtIKManager.Instance.CtrlDic)
            {
                kv.Value.UpateIKConfig();
            }
        }
#endif
        
    }
}