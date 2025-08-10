using UnityEngine;

public class AnimationEntryController : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("enter");
        animator.GetComponent<IAnimationEntryActionNotify>()?.OnEntryMoment();
    }
}
