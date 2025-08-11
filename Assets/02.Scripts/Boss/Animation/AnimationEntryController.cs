using UnityEngine;

public class AnimationEntryController : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<IAnimationEntryActionNotify>()?.OnEntryMoment();
    }
}
