using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IKAnimation
{
    [HelpURL("https://zhuanlan.zhihu.com/p/161106076")]
    [CreateAssetMenu(fileName = "New LookAtIK Config", menuName = "Final IK/Create LookAt Config")]
    public class LookAtIKConfig : SerializedScriptableObject
    {
        [Title("视野参数")]
        [LabelText("视角范围"), Range(0, 180)]
        public float        Angle;
        
        [LabelText("视野距离")]
        public float        ViewDistance     = 1000;
        
        [Title("全局动画曲线类型(默认均速)")]
        [LabelText("看向")]
        public Ease         LookCurveType    = Ease.Linear;
        
        [LabelText("回正")]
        public Ease         ResetCurveType   = Ease.Linear;

        [Title("角度时间相关")]
        [PropertySpace]
        [LabelText("锁定目标默认时间")] 
        public float        FadeInTime       = 0.5f;
        
        [LabelText("目标丢失默认回正时间")] 
        public float        FadeOutTime      = 0.5f;
        
        [LabelText("锁定转向所需时间(没有配置使用默认时间)")]
        public List<StepTime> DirAngelTimeList;
        
        [Title("锁定目标切换相关")]
        [LabelText("切换锁定目标回正?")]
        public bool         ST4Forward;
        
        [LabelText("切换目标回正时间")] 
        [ShowIf("@ST4Forward == true")]
        public float        SwitchFadeOutTime= 0.5f;
        
        [LabelText("直接转向所需时间(没有配置使用默认时间)")]
        [ShowIf("@ST4Forward == false")]
        public List<StepTime> AngelTimeList;

        [LabelText("直接转向默认时间")]
        [ShowIf("@ST4Forward == false")]
        public float        DireTurnToTime   = 1f;
        
        [LabelText("切换动画所需最小角度"), Range(0, 360)]
        public float        TargetSwithAngle = 5f;
        
        [Title("权重")]
        [PropertySpace]
        [LabelText("全局权重"), Range(0, 1)] 
        public float        Weight           = 1;
        
        [LabelText("头权重"), Range(0, 1)] 
        public float        HeadWeight       = 1f;
        
        [LabelText("身体权重"), Range(0, 1)] 
        public float        BodyWeight       = 0;
        
        [LabelText("眼睛权重"), Range(0, 1)] 
        public float        EyesWeight       = 0;
        
        [Range(0, 1), HideInInspector] 
        public float        ClampWeight      = 0.5f;
        
        [Range(0, 1), HideInInspector] 
        public float        ClampHeadWeight  = 0.5f;
        
        [Range(0, 1), HideInInspector] 
        public float        ClampEyesWeight  = 0.5f;

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var kv in LooKAtIKManager.Instance.CtrlDic)
            {
                kv.Value.UpdateModelCtrl();
            }
        }
#endif
        
    }
    
    [Serializable]
    public class StepTime
    {
        [LabelText("角度"), Range(0, 360)]
        public float AngleValue;

        [LabelText("时间"), Range(0, 10)]
        public float Time;
    }
}