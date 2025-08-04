using System.Collections.Generic;
using System.Linq;
using Fusion.Addons.FSM;
using RaycastPro.Detectors;
using UnityEngine;

public class BerserkChase : ABerserkSubStateBase
{
    private Transform _target;
    private float _moveSpeed;
    private float _sprintMultipler;
    private float _enemySearchTimer;
    private RangeDetector _rangeDetector;
    public BerserkChase(PlayerFSM controller) : base(controller) {
        AnimState = "Run";
        _rangeDetector = _fsm.GetComponent<RangeDetector>();
    }

    private List<PlayerFSM> _allPlayers;

    private void CachePlayers()
    {
        _allPlayers = GameObject.FindObjectsByType<PlayerFSM>(FindObjectsSortMode.None).ToList();
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
        CachePlayers();
    }

    protected override void OnEnterStateRender()
    {
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }
    protected override void OnFixedUpdate()
    {
        _enemySearchTimer += _fsm.Runner.DeltaTime;

        if (_enemySearchTimer > 1.0f && (_target == null || !IsValid(_target)))
        {
            _enemySearchTimer = 0f;
            _target = FindClosestEnemy();

            if (_target == null)
            {
                KCC.Move(Vector3.zero);
                return;
            }
        }

        if (_target == null)
        {
            KCC.Move(Vector3.zero);
            return;
        }

        if (CanStartAttack())
        {
            Machine.ForceActivateState<BerserkAttack>();
            return;
        }

        Vector3 dir = (_target.position - _fsm.transform.position).normalized;
        KCC.SetLookRotation(Quaternion.LookRotation(dir));
        KCC.Move(dir * _moveSpeed * _sprintMultipler);
    }


    protected Transform FindClosestEnemy()
    {
        Transform closest = null;
        float minDist = float.MaxValue;
        float maxSearchRadius = 20f;

        foreach (var go in _allPlayers)
        {
            if (go == _fsm) continue;
            if (go.IsDead) continue;

            float dist = Vector3.Distance(_fsm.transform.position, go.transform.position);
            if (dist > maxSearchRadius) continue;

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
