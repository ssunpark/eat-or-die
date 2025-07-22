using UnityEngine;
using Fusion;

public class PlayerAttackState : APlayerState
{
    public PlayerAttackState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller) { }

    public override void Enter()
    {
        _fsm.StartCoroutine(AttackCoroutine());
        if (_controller.Object.HasInputAuthority)
        {
            // 애니메이션 트리거
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Attack);

            _controller.SetAttackingLocal(true);
        }

    }

    private System.Collections.IEnumerator AttackCoroutine()
    {
        float damage = _stat.GetStat(EStatType.Damage);
        float attackSpeed = _stat.GetStat(EStatType.AttackSpeed);
        float attackDelay = 1f / Mathf.Max(attackSpeed, 0.01f);



        yield return new WaitForSeconds(0.1f);

        Vector3 attackOrigin = _controller.transform.position + Vector3.up * 0.5f;
        Vector3 direction = _controller.transform.forward;

        if (Physics.Raycast(attackOrigin, direction, out RaycastHit hit, 1.5f))
        {
            if (hit.collider.TryGetComponent(out NetworkObject target))
            {
                _controller.RPC_DealDamage(target, Mathf.RoundToInt(damage));
                Debug.Log($"Attacked {target.name} for {damage} damage.");
            }
        }
        Debug.Log(attackDelay);
        yield return new WaitForSeconds(attackDelay);
        Debug.Log("gogo");
        _fsm.ChangeState(EPlayerState.Idle);
    }

    public override void Tick() { } // 무시

    public override void Exit()
    {
        if (_controller.Object.HasInputAuthority)
            _controller.SetAttackingLocal(false);
    }
}
