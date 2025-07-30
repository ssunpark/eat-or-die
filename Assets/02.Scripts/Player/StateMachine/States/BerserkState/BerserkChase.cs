using UnityEngine;
using Fusion.Addons.FSM;

public class BerserkChase : ABerserkSubStateBase
{
    private Transform _target;

    public BerserkChase(PlayerController controller) : base(controller) { }

    protected override void OnEnterState()
    {
        _target = FindClosestEnemy();
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

        float dist = Vector3.Distance(_controller.transform.position, _target.position);
        if (dist < _controller.Stat.GetStat(EStatType.AttackRange))
        {
            Machine.ForceActivateState<BerserkAttack>();
        }
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
