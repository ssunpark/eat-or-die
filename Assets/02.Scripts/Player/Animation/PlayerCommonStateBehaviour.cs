using UnityEngine;

public class PlayerCommonStateBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerAnimator>()?.OnAnimationFinished();
    }
}
