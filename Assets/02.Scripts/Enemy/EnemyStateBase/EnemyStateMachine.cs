using Fusion.Addons.FSM;
using UnityEngine;

public class EnemyStateMachine : StateMachine<AEnemyState>
{
    public EnemyStateMachine(string name, AEnemyStateBehaviour parentBehaviour, params AEnemyState[] states) : base(name, states)
    {
        foreach (AEnemyState state in states)
        {
            state.ParentBehaviour = parentBehaviour;
        }
    }
}