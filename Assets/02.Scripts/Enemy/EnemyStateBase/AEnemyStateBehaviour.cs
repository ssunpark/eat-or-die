using Fusion.Addons.FSM;
using UnityEngine;
using UnityEngine.AI;

public abstract class AEnemyStateBehaviour : StateBehaviour<AEnemyStateBehaviour>
{
    // 어느 상태에서나 강제로 전이되어야 하는 상태인지 확인하는 bool 값이 추가될 수 있음
    public new EnemyBehaviourMachine Machine => base.Machine as EnemyBehaviourMachine;
}