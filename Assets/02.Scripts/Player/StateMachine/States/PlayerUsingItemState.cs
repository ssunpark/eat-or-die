using UnityEngine;
using Fusion; // Add Fusion for NetworkInputData

public class PlayerUsingItemState : APlayerState
{
    public PlayerUsingItemState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {

    }
    public override void Enter()
    {
        if (_controller.Object.HasInputAuthority)
        {
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Interact);

            // 애니메이션 이벤트?로 할 예정
            _fsm.Interact.UseOrInteract(usable: _fsm.Usable);

            _controller.RPC_SetMoveFlag(true);
        }
    }
    private float _time = 0f;
    public override void Tick()
    {
        _time += _fsm.Runner.DeltaTime;
        // 애니메이션 이벤트로 상호작용이 끝났는지 확인할 예정
        if (_time >= 1f) // 예시로 1초 후에 상태를 변경
        {
            _fsm.ChangeState(EPlayerState.Idle);
            _time = 0f; // 타이머 초기화
        }
    }

    public override void Exit()
    {
        if (_controller.Object.HasInputAuthority)
            _controller.RPC_SetMoveFlag(false);
    }
}