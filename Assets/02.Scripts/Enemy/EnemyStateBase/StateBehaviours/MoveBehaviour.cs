using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

public class MoveBehaviour : AEnemyStateBehaviour
{
    private EnemyStateMachine _moveStateMachine;
    
    private TraceState _traceState = new TraceState();
    // private AvoidState _avoidState;

    protected override void OnCollectChildStateMachines(List<IStateMachine> stateMachines)
    {
        AEnemyState[] stateList = new AEnemyState[]
        {
            _traceState,
            // _avoidState
        };
        _moveStateMachine = new EnemyStateMachine("Move State Machine", this, stateList);
        
        stateMachines.Add(_moveStateMachine);
    }

    protected override bool CanEnterState()
    {
        return (Machine.Context.Target != null);
    }

    protected override void OnEnterState()
    {
        Debug.Log("Moving...");
        _moveStateMachine.TryActivateState(_traceState);
    }
}