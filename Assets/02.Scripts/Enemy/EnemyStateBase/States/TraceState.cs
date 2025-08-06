using Fusion.Addons.FSM;
using UnityEngine;

public class TraceState : AEnemyState
{
    private static readonly int MoveForward = Animator.StringToHash("MoveForward");
    
    protected override bool CanEnterState()
    {
        return (Context.Target != null);
    }

    protected override void OnEnterState()
    {
        Debug.Log("Trace...");
        Context.Agent.isStopped = false;
        Context.Animator.SetTrigger(MoveForward);
    }

    protected override void OnFixedUpdate()
    {
        if (ParentBehaviour.Machine.TryActivateState<AttackBehaviour>())
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
    }
}