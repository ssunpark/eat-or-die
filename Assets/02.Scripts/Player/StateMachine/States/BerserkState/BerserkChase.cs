using UnityEngine;
using Fusion.Addons.FSM;

public class BerserkChase : ABerserkSubStateBase
{
    private Transform _target;

    public BerserkChase(PlayerController controller) : base(controller) { }
    protected override void OnInitialize()
    {
        this.AddTransition(
            Machine.GetState<BerserkAttack>(),
            CanStartAttack
        );
    }

    private bool CanStartAttack()
    {
        if (_target == null) return false;
        if (!_controller.HasStateAuthority) return false;

        float distance = Vector3.Distance(_controller.transform.position, _target.position);
        if (distance > _stat.GetStat(EStatType.AttackRange)) return false;

        float cooldown = Mathf.Max(1f / _stat.GetStat(EStatType.AttackSpeed), 0.01f);
        return _controller.LastAttackTime + cooldown < Machine.Runner.LocalRenderTime;
    }
    protected override void OnEnterState()
    {
        _target = FindClosestEnemy();
    }

    protected override void OnExitState()
    {
        _controller.Movement.Move(Vector3.zero, false);
    }

    protected override void OnFixedUpdate()
    {
        if (_target == null || !IsValid(_target))
        {
            _target = FindClosestEnemy();

            _controller.Movement.Move(Vector3.zero, false);
            return;
        }

        Vector3 dir = (_target.position - _controller.transform.position).normalized;
        _controller.Movement.Move(dir, true);

    }

    // 일단 플레이어만 찾게
    protected Transform FindClosestEnemy()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var go in players)
        {
            if (go == _controller.gameObject) continue;

            float dist = Vector3.Distance(_controller.transform.position, go.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = go.transform;
            }
        }

        return closest;
    }

    private bool IsValid(Transform t)
    {
        return t != null;
    }
}
