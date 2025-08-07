using Fusion.Addons.FSM;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviourMachine : StateMachine<AEnemyStateBehaviour>
{
    public EnemyContext Context;
    
    public EnemyBehaviourMachine(string name, EnemyContext context, params AEnemyStateBehaviour[] states) : base(name, states)
    {
        Context = context;
    }
}