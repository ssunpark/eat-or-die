using Fusion.Addons.FSM;
using UnityEngine;

public class TraceState : AEnemyState
{
    protected override bool CanEnterState()
    {
        return (Context.Target != null);
    }

    protected override void OnFixedUpdate()
    {
        Context.Agent.SetDestination(Context.Target.transform.position);
    }
}