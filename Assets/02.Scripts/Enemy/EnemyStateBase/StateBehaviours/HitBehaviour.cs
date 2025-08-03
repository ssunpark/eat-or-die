using System;
using Fusion.Addons.FSM;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class HitBehaviour : AEnemyStateBehaviour
{
    private static readonly int Hit = Animator.StringToHash("Hit");
    
    protected override void OnEnterState()
    {
        Debug.Log("Hit...");
        Machine.Context.Animator.SetTrigger(Hit);
    }

    protected override void OnFixedUpdate()
    {
        AnimatorStateInfo stateInfo = Machine.Context.Animator.GetCurrentAnimatorStateInfo(0);

        if (!Machine.Context.Animator.IsInTransition(0) && stateInfo.normalizedTime >= 1f)
        {
            Machine.TryActivateState<IdleBehaviour>();
        }
    }

    protected override bool CanExitState(AEnemyStateBehaviour nextStateBehaviour)
    {
        if (nextStateBehaviour is IdleBehaviour) return true;

        return false;
    }
}