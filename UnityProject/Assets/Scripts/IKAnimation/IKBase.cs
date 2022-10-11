using UnityEngine;

namespace IKAnimation
{
    public class IKBase : MonoBehaviour
    {
        public bool                 IKActive;

        public virtual void OpenIK()
        {
            this.IKActive = true;
        }

        public virtual void CloseIK()
        {
            this.IKActive = false;
        }
    }
}