using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class PlayerSMB : StateMachineBehaviour
    {
        public float damping = 0.5f;

        private readonly int hashHorizontalParameter = Animator.StringToHash("Horizontal");
        private readonly int hashVerticalParameter = Animator.StringToHash("Vertical");

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetFloat(hashHorizontalParameter, animator.GetFloat("Horizontal"), damping, Time.deltaTime);
            animator.SetFloat(hashVerticalParameter, animator.GetFloat("Vertical"), damping, Time.deltaTime);
        }
    }
}