using Fusion.Addons.FSM;
using UnityEngine;
using UnityEngine.AI;

public abstract class AEnemyStateBehaviour : StateBehaviour<AEnemyStateBehaviour>
{
    public new EnemyBehaviourMachine Machine => base.Machine as EnemyBehaviourMachine;
}