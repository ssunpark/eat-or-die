using Fusion.Addons.FSM;
using UnityEngine;

public class TraceState : AEnemyState
{
    protected override bool CanEnterState()
    {
        return (Context.Target != null);
    }

    protected override void OnEnterState()
    {
        Context.Agent.isStopped = false;
        Context.Owner.AnimationState = EAnimationState.RunForward;
    }

    protected override void OnFixedUpdate()
    {
        if (ParentBehaviour.Machine.TryActivateState<AttackBehaviour>(true))
        {
            return;
        }
        
        Context.Agent.SetDestination(Context.Target.transform.position);
        
        if (!Context.Animator.IsInTransition(0))
        {
            Context.Mover.Move();
        }
    }

    protected override void OnExitState()
    {
        Context.Agent.isStopped = true;
        Context.Agent.velocity = Vector3.zero;
    }
}