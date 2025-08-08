using Fusion.Addons.FSM;
using UnityEngine;

public class HitBehaviour : AEnemyStateBehaviour
{
    protected override void OnEnterState()
    {
        Machine.Context.Owner.AnimationState = EAnimationState.Hit;
    }

    protected override void OnFixedUpdate()
    {
        AnimatorStateInfo stateInfo = Machine.Context.Animator.GetCurrentAnimatorStateInfo(0);

        if (!Machine.Context.Animator.IsInTransition(0) && stateInfo.normalizedTime >= 1f)
        {
            if (Machine.PreviousState is AttackBehaviour or HitBehaviour)
            {
                Machine.TryActivateState<IdleBehaviour>();
            }
            else
            {
                Machine.TryActivateState(Machine.PreviousState, true);
            }
        }
    }

    protected override bool CanExitState(AEnemyStateBehaviour nextStateBehaviour)
    {
        return nextStateBehaviour is not AttackBehaviour;
    }
}