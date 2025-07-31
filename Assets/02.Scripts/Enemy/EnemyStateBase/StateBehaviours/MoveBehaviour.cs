using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

public class MoveBehaviour : AEnemyStateBehaviour
{
    private EnemyStateMachine _moveStateMachine;
    
    [SerializeField] private TraceState _traceState = new TraceState();
    // [SerializeField] private AvoidState _avoidState;

    protected override void OnCollectChildStateMachines(List<IStateMachine> stateMachines)
    {
        AEnemyState[] stateList = new AEnemyState[]
        {
            _traceState,
            // _avoidState
        };
        _moveStateMachine = new EnemyStateMachine("Move State Machine", this, stateList);
    }
    
    protected override void OnEnterStateRender()
    {
        Debug.Log("Moving...");
    }
    
    protected override void OnFixedUpdate()
    {
        Machine.Context.Mover.Move();
    }
}