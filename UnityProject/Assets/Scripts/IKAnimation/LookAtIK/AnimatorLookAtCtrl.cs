using UnityEngine;

namespace IKAnimation
{
    public class AnimatorLookAtCtrl : StateMachineBehaviour
    {
        private LookAtIKCtrl IKCtrl;
    
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.IKCtrl == null)
                this.IKCtrl = animator.GetComponent<LookAtIKCtrl>();
            if (this.IKCtrl != null)
            {
                switch (this.IKCtrl)
                {
                    case MonsterLookAtIKCtrl rMctrl:
                        rMctrl.OpenIK();
                        break;
                    case HumanLookAtIKCtrl rHctrl:
                        rHctrl.OpenIK();
                        break;
                }
            }
        }
    
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.IKCtrl == null)
                this.IKCtrl = animator.GetComponent<LookAtIKCtrl>();
            if (this.IKCtrl != null)
            {
                switch (this.IKCtrl)
                {
                    case MonsterLookAtIKCtrl rMctrl:
                        rMctrl.CloseIK();
                        break;
                    case HumanLookAtIKCtrl rHctrl:
                        rHctrl.CloseIK();
                        break;
                }
            }
        }
    
    }
}

