using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
public static class PlayerFSMTransitionEvaluator
{
    public static bool Evaluate(PlayerController controller, NetworkInputData input, NetworkRunner runner, out APlayerStateBase nextState)
    {
        nextState= null;

        controller.StateValues.Interactable = null;
        controller.StateValues.Usable = null;
        // 공격
        if (CanAttack(controller, runner))
        {
            if (input.isAttacking)
            {
                nextState = controller.FSMStateInstances.Attack;
                return true;
            }
        }

        // 상호작용
        if (input.isInteracting && controller.Interact.TryInteract(out var interactable))
        {
            controller.StateValues.Interactable = interactable;
            nextState = controller.FSMStateInstances.Interact;
            return true;
        }

        // 아이템 사용
        if (input.isUsing && controller.Interact.TryUseItem(out var usable))
        {
            controller.StateValues.Usable = usable;
            nextState = controller.FSMStateInstances.UseItem;
            return true;
        }

        // 이동
        if (input.direction.sqrMagnitude > 0.01f)
        {
            nextState = controller.FSMStateInstances.Move;
            return true;
        }

        return false;
    }
    public static bool CanAttack(PlayerController controller, NetworkRunner runner)
    {
        float atkSpd = controller.Stat.GetStat(EStatType.AttackSpeed);
        return controller.LastAttackTime + 1f / Mathf.Max(atkSpd, 0.01f) < runner.LocalRenderTime;
    }
}