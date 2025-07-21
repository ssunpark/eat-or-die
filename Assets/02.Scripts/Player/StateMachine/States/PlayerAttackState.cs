using UnityEngine;
using Fusion;

public class PlayerAttackState : PlayerStateBase
{
    public PlayerAttackState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller) { }

    public override void Enter()
    {
        _fsm.StartCoroutine(AttackCoroutine());
    }

    private System.Collections.IEnumerator AttackCoroutine()
    {
        float damage = _stat.GetStat(EStatType.Damage);
        float attackSpeed = _stat.GetStat(EStatType.AttackSpeed);
        float attackDelay = 1f / Mathf.Max(attackSpeed, 0.01f);

        // 애니메이션 트리거
        _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Attack);

        yield return new WaitForSeconds(0.1f);

        if (_controller.GetInput(out NetworkInputData inputData))
        {
            Vector3 attackOrigin = _controller.transform.position + Vector3.up * 0.5f;
            Vector3 direction = _controller.transform.forward;

            if (Physics.Raycast(attackOrigin, direction, out RaycastHit hit, 1.5f))
            {
                if (hit.collider.TryGetComponent(out NetworkObject target))
                {
                    _controller.RPC_DealDamage(target, Mathf.RoundToInt(damage));
                }
            }
        }

        yield return new WaitForSeconds(attackDelay);

        _fsm.ChangeState(EPlayerState.Idle);
    }

    public override void Tick() { } // 무시
}
