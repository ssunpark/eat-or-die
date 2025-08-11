using Fusion.Addons.FSM;
using UnityEngine;

public class HitBehaviour : AEnemyStateBehaviour
{
    protected override void OnEnterState()
    {
        Debug.Log("Hit!");
        Machine.Context.Owner.AnimationState = EAnimationState.Hit;
    }

    protected override void OnFixedUpdate()
    {
        AnimatorStateInfo stateInfo = Machine.Context.Animator.GetCurrentAnimatorStateInfo(0);

        if (!Machine.Context.Animator.IsInTransition(0) && stateInfo.normalizedTime >= 1f)
        {
            Machine.TryActivateState<IdleBehaviour>();
        }
    }

    protected override bool CanExitState(AEnemyStateBehaviour nextStateBehaviour)
    {
        return nextStateBehaviour is IdleBehaviour;
    }

    protected override void OnExitState()
    {
        Debug.Log("Hit Exit!");
    }
}