using Fusion.Addons.FSM;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviourMachine : StateMachine<EnemyStateBehaviour>
{
    public EnemyBehaviourMachine(string name, params EnemyStateBehaviour[] states) : base(name, states)
    {
    }
}