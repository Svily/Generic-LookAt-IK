using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IKAnimation
{
    public class LookAtIKModelCtrl : MonoBehaviour
    {
        public List<LookAtIKCtrl> IKCtrlList = new List<LookAtIKCtrl>();

        public GameObject LookAtPoint;
        
        private void Awake()
        {
            this.IKCtrlList = this.transform.GetComponentsInChildren<LookAtIKCtrl>().ToList();
            LooKAtIKManager.Instance.AddIKCtrl(this.transform.name, this);
            if (this.LookAtPoint == null)
            {
                this.LookAtPoint = new GameObject("LookAtPoint");
                this.LookAtPoint.transform.SetParent(this.transform);
            }
            
            this.InitIKCtrl();
        }


        private void InitIKCtrl()
        {
            this.IKCtrlList.ForEach((x) =>
            {
                x.InitLookAtIK(this.transform, this.LookAtPoint);
            });
        }
        
        public void SetLookAtTarget(GameObject rGo)
        {
            this.SetLookAtTarget(rGo?.transform);
        }
        
        public void SetLookAtTarget(Transform rTrans)
        {
            if (rTrans == null)
            {
                return;
            }
            this.IKCtrlList.ForEach((x) =>
            {
                x.SetLookAtTarget(rTrans);
            });
        }

        public void SetLookAtNull()
        {
            this.IKCtrlList.ForEach((x) =>
            {
                x.SetLookAtNull();
            });
            
        }
        
        public void UpdateModelCtrl()
        {
            this.IKCtrlList.ForEach((x)=>{x.UpateIKConfig();});
        }


        private void OnDestroy()
        {
            LooKAtIKManager.Instance.RemoveIKCtrl(this.transform.name);
            Destroy(this.LookAtPoint);
        }
    }
}