using Fusion; // Add Fusion for NetworkInputData
using Fusion.Addons.FSM; // Add Fusion FSM for PlayerStateMachine
using UnityEngine;

public class PlayerIdleState : APlayerStateBase
{
    public PlayerIdleState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.Idle;
    }
    protected override void OnInitialize()
    {
        // 전이: 방향 입력 있으면 Move로
        this.AddTransition(
            _controller.FSMStateInstances.Move,
            () => _controller.GetInput(out NetworkInputData input) && input.direction.sqrMagnitude > 0.01f
        );

        // 전이: 공격 버튼 입력 시 Attack으로
        this.AddTransition(
            _controller.FSMStateInstances.Attack,
            () => CanStartAttack()
        );

        // 전이: 인터랙션 키 누르면 Interact로
        this.AddTransition(
            _controller.FSMStateInstances.Interact,
            () => _controller.GetInput(out NetworkInputData input) && input.isInteracting && _controller.CanInteract(out _)
        );

        // 전이: 아이템 사용
        this.AddTransition(
            _controller.FSMStateInstances.UseItem,
            () => _controller.GetInput(out NetworkInputData input) && input.isUsing && _controller.CanUseHeldItem(out _)
        );
    }

    private bool CanStartAttack()
    {
        if(!_controller.GetInput(out NetworkInputData input))
        {
            return false;
        }

        if (!input.isAttacking) return false;

        float cooldown = Mathf.Max(1f / _stat.GetStat(EStatType.AttackSpeed), 0.01f);
        return _controller.LastAttackTime + cooldown < Machine.Runner.LocalRenderTime;
    }


}