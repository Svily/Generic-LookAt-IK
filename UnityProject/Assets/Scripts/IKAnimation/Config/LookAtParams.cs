using Sirenix.OdinInspector;
using UnityEngine;

namespace IKAnimation
{
    [CreateAssetMenu(fileName = "New LookAtParam Config", menuName = "Final IK/Create LookAt Param Config")]
    public class LookAtParams : ScriptableObject
    {
        
        [Title("头骨骼轴向修正")]
        [LabelText("是否修正轴向")]
        public bool         FixHeadAxis      = false;
        
        [LabelText("偏移角度"), Range(-180, 180),ShowIf("@FixHeadAxis == true")]
        public float        OffsetAngle      = 30;
        
        [LabelText("旋转轴"), ShowIf("@FixHeadAxis == true && CustomAxis == false")]
        public AxisType     AxisType         = AxisType.Left;
        
        [LabelText("自定义旋转轴"), ShowIf("@FixHeadAxis == true")]
        public bool         CustomAxis       = false;
        
        [LabelText("旋转轴"), ShowIf("@CustomAxis == true && FixHeadAxis == true")]
        public Vector3      AxisVec          = Vector3.zero;
        
        
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
            if (LooKAtIKManager.instance?.CtrlDic == null)
                return;
            foreach (var kv in LooKAtIKManager.instance?.CtrlDic)
            {
                kv.Value?.InitIKSolver();
            }
        }
#endif
    }
}