using Fusion.Addons.FSM;
using UnityEngine;

public abstract class AEnemyState : State<AEnemyState>
{
    public AEnemyStateBehaviour ParentBehaviour;
    public EnemyContext Context => ParentBehaviour.Machine.Context;
}