using Fusion.Addons.FSM;
using UnityEngine;

public class AttackActionState : AEnemyState
{
    protected override void OnEnterState()
    {
        Context.Owner.AnimationState = EAnimationState.Attack;
    }

    protected override void OnFixedUpdate()
    {
        AnimatorStateInfo stateInfo = Context.Animator.GetCurrentAnimatorStateInfo(0);
        
        if (!Context.Animator.IsInTransition(0) && stateInfo.normalizedTime >= 1f)
        {
            AttackBehaviour behaviour = ParentBehaviour as AttackBehaviour;
            if (behaviour != null)
            {
                behaviour.IsAttackFinished = true;
                behaviour.Machine.TryActivateState<IdleBehaviour>();
            }
        }
    }
}