using UnityEngine;
using Fusion;

public class PlayerAttackState : APlayerState
{
    public PlayerAttackState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller) { }


    private float _damage;
    private float _attackSpeed;
    private float _attackDelay;
    private float _attackTimer;

    public override void Enter()
    {
        if (_controller.Object.HasInputAuthority)
        {
            // 애니메이션 트리거
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Attack);

            _controller.RPC_SetMoveFlag(true);
        }

        _damage = _stat.GetStat(EStatType.MeleeDamage);
        _attackSpeed = _stat.GetStat(EStatType.AttackSpeed);
        _attackDelay = 0.6f / Mathf.Max(_attackSpeed, 0.01f);
        _controller.LastAttackTime = _fsm.Runner.LocalRenderTime;

        // 애니메이션 이벤트로 실행될 부분
        Vector3 attackOrigin = _controller.transform.position + Vector3.up * 0.5f;
        Vector3 direction = _controller.transform.forward;

        if (Physics.Raycast(attackOrigin, direction, out RaycastHit hit, _stat.GetStat(EStatType.AttackRange)))
        {
            if (hit.collider.TryGetComponent(out NetworkObject target))
            {
                _controller.RPC_DealDamage(target, Mathf.RoundToInt(_damage));
            }
        }
        //=================================
    }

    public override void Tick() 
    {
        _attackTimer += _fsm.Runner.DeltaTime;
        if (_attackTimer >= _attackDelay)
        {
            _attackTimer = 0f;
            _fsm.ChangeState(EPlayerState.Idle);
        }
    }

    public override void Exit()
    {
        if (_controller.Object.HasInputAuthority)
            _controller.RPC_SetMoveFlag(false);
    }
}
