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
        this.AddTransition(_controller.FSMStateInstances.Move, CanMove);

        // 전이: 공격 버튼 입력 시 Attack으로
        this.AddTransition(_controller.FSMStateInstances.Attack, CanStartAttack);

        // 전이: 인터랙션 키 누르면 Interact로
        this.AddTransition(_controller.FSMStateInstances.Interact, CanInteract);

        // 전이: 아이템 사용
        this.AddTransition(_controller.FSMStateInstances.UseItem, CanUseItem);
    }
    protected override void OnEnterState()
    {
        
    }
    private bool CanMove()
    {
        if (!_controller.HasStateAuthority) return false;

        if (!TryCacheInput()) return false;

        if (_input.direction.sqrMagnitude <= 0.01f) return false;

        return true;
    }
    


}