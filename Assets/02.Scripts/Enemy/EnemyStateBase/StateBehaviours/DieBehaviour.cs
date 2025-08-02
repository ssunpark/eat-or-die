using Fusion;
using UnityEngine;
using Fusion.Addons.FSM;

public class DieBehaviour : AEnemyStateBehaviour
{
    [SerializeField] private float _despawnTime = 2f;
    private static readonly int Die = Animator.StringToHash("Die");

    protected override void OnEnterState()
    {
        Machine.Context.Animator.SetTrigger(Die);
    }

    protected override void OnFixedUpdate()
    {
        if (Machine.StateTime >= _despawnTime)
        {
            NetworkObject owner = GetComponentInParent<NetworkObject>();
            Runner.Despawn(owner);
        }
    }

    protected override bool CanExitState(AEnemyStateBehaviour nextState)
    {
        return false;
    }
}
