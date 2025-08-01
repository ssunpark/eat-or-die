using UnityEngine;
using Fusion.Addons.FSM;

public class SpawnBehaviour : AEnemyStateBehaviour
{
    private static readonly int Spawn = Animator.StringToHash("Spawn");
    private bool _isSpawned = false;

    protected override void OnFixedUpdate()
    {
        AnimatorStateInfo stateInfo = Machine.Context.Animator.GetCurrentAnimatorStateInfo(0);

        if (!Machine.Context.Animator.IsInTransition(0) && stateInfo.normalizedTime >= 1f)
        {
            _isSpawned = true;
            Machine.TryActivateState<IdleBehaviour>();
        }
    }

    protected override bool CanExitState(AEnemyStateBehaviour nextStateBehaviour)
    {
        return _isSpawned;
    }

    protected override void OnExitState()
    {
        // Machine.Context.Animator.SetBool(Spawn, false);
    }
}