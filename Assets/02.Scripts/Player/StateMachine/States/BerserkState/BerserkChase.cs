using UnityEngine;
using Fusion.Addons.FSM;

public class BerserkChase : ABerserkSubStateBase
{
    private Transform _target;
    private float _moveSpeed;
    private float _sprintMultipler;
    public BerserkChase(PlayerFSM controller) : base(controller) {
        AnimState = "Run";

    }

    private bool CanStartAttack()
    {
        if (_target == null) return false;
        if (!_fsm.HasStateAuthority) return false;

        float distance = Vector3.Distance(_fsm.transform.position, _target.position);
        if (distance > _stat.GetStat(EStatType.AttackRange)) return false;

        float cooldown = Mathf.Max(1f / _stat.GetStat(EStatType.AttackSpeed), 0.01f);
        return _fsm.LastAttackTime + cooldown < Machine.Runner.LocalRenderTime;
    }
    protected override void OnEnterState()
    {
        if (_stat == null)
        {
            _stat = _fsm.PlayerNetworkObject.Stat;
        }
        if (_resource == null)
        {
            _resource = _fsm.PlayerNetworkObject.Resource;
        }

        if (_stat == null || _resource == null)
        {
            Debug.LogError("PlayerMoveState: Stat or Resource is null. Cannot enter state.");
            return;
        }
        _moveSpeed = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.MoveSpeed);
        _sprintMultipler = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.SprintingMultiplier);
        _target = FindClosestEnemy();
    }

    protected override void OnEnterStateRender()
    {
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }
    protected override void OnFixedUpdate()
    {
        if (_target == null || !IsValid(_target))
        {
            _target = FindClosestEnemy();

            KCC.Move(Vector3.zero);
            return;
        }

        if(CanStartAttack())
        {
            Machine.ForceActivateState<BerserkAttack>();
            return;

        }

        Vector3 dir = (_target.position - _fsm.transform.position).normalized;

        KCC.SetLookRotation(Quaternion.LookRotation(dir));
        KCC.Move(dir * _moveSpeed * _sprintMultipler);
    }

    // 일단 플레이어만 찾게
    protected Transform FindClosestEnemy()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var go in players)
        {
            if (go == _fsm.gameObject) continue;

            float dist = Vector3.Distance(_fsm.transform.position, go.transform.position);
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
