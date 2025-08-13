using Fusion.Addons.FSM;
using UnityEngine;

public class TraceState : AEnemyState
{
    protected override void OnEnterState()
    {
        Debug.Log("Enter Trace State");
        Context.Agent.isStopped = false;
        Context.Owner.AnimationState = EAnimationState.RunForward;
    }

    protected override void OnFixedUpdate()
    {
        if (Context.Animator.IsInTransition(0)) return;
        
        Context.Agent.SetDestination(Context.Target.transform.position);
        
        Context.Mover.Move();
        
        ParentBehaviour.Machine.TryActivateState<AttackBehaviour>();
    }

    protected override void OnExitState()
    {
        Context.Agent.isStopped = true;
        Context.Agent.velocity = Vector3.zero;
    }
}