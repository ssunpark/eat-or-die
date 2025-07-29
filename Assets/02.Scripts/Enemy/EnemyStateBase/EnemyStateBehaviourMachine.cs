using Fusion.Addons.FSM;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateBehaviourMachine : StateMachine<EnemyStateBehaviour>
{
    public EnemyStateBehaviourMachine(string name, params EnemyStateBehaviour[] states) : base(name, states)
    {
        foreach (EnemyStateBehaviour stateBehaviour in states)
        {
            stateBehaviour.Machine = this;
        }
    }
}