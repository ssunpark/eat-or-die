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
        Context.Animator.SetTrigger(MoveForward);
    }

    protected override void OnFixedUpdate()
    {
        Vector3 toTarget = Context.Target.transform.position - ParentBehaviour.transform.position;
        float distance = toTarget.magnitude;

        if (distance <= Context.Stat.AttackRange
            && Vector3.Angle(toTarget, ParentBehaviour.transform.forward) <= Context.Stat.AttackAngle)
        {
            ParentBehaviour.Machine.TryActivateState<AttackBehaviour>();
        }
        
        Context.Agent.SetDestination(Context.Target.transform.position);
    }
}