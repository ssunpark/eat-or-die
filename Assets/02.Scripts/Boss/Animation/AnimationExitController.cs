using UnityEngine;

public class AnimationExitController : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<IAnimationExitActionNotify>()?.OnExitMoment();
    }
}
