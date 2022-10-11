using Sirenix.OdinInspector;
using UnityEngine;

namespace IKAnimation
{
    public class LookAtIKTest : MonoBehaviour
    {

        [LabelText("目标1")]
        public GameObject Target1;
        
        [LabelText("目标2")]
        public GameObject Target2;

        public LookAtIKCtrl IKCtrl;

        private int SwitchCount = 0;

        private void Awake()
        {
            this.IKCtrl = this.GetComponent<LookAtIKCtrl>();
            this.Target1 = new GameObject("Target1");
            this.Target2 = new GameObject("Target2");
            this.Target1.transform.SetParent(this.transform);
            this.Target1.transform.localPosition = new Vector3(-18f, 0f, 11f);
            this.Target2.transform.SetParent(this.transform);
            this.Target2.transform.localPosition = new Vector3(25f, 0f, 11f);
        }
        
        [Button("测试")]
        public  void TestSetTarget()
        {
            this.IKCtrl.SetLookAtTarget(this.Target1);
        }

        [Button("切换目标")]
        public void SwitchTarget()
        {
            this.SwitchCount++;
            this.IKCtrl.SetLookAtTarget(this.SwitchCount % 2 == 0? this.Target1 : this.Target2);
        }

        [Button("置空")]
        public void SetNull()
        {
            this.IKCtrl.SetLookAtNull();

        }
    }
}