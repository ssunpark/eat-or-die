using Mono.Cecil;
using UnityEngine;

public class PlayerHitState : PlayerStateBase
{
    private float _hitStunDuration;
    private float _elapsed;

    public PlayerHitState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {
    }

    public override void Enter()
    {
        _elapsed = 0f;
        _hitStunDuration = 0.5f; // 경직 시간 (애니메이션 길이에 맞춰 조정)

        _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Hit);

    }

    public override void Tick()
    {
        _elapsed += Time.deltaTime;

        if (_elapsed >= _hitStunDuration)
        {
            _fsm.ChangeState(EPlayerState.Idle);
        }
    }

    public override void Exit()
    {
    }
}
