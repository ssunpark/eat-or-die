using System;
using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class DragonState_Idle : DragonStateBase, IParentState, IAnimationExitActionNotify
{
    private StateMachine<DragonSubStateBase> _subStateMachine;
    private DragonStateParameterSet.BaseParams _baseParams;
    
    private bool _alerted;
    private bool _roared;

    public DragonState_Idle(DragonContext context) : base(context)
    {
        _baseParams = Context.Parameter.Base;
    }

    protected override void OnEnterState()
    {
        Context.Movement.SetNavMeshAgentMoveData(_baseParams.MoveSpeed, _baseParams.RotationSpeed);

        Context.Combat.SetFightMode(false);

        Context.Sight.SetSightDetector(_baseParams.FullAwarenessRadius, _baseParams.DetectRadius, _baseParams.FOVAngle);

        TryActiveRandomSubState();
    }

    protected override void OnFixedUpdate()
    {
        // 이미 타겟 있으면 Alert 전환
        if (!_roared && Context.Sight.HasTarget)
        {
            Context.Combat.SetFightMode(true);
            Context.Animator.SetTrigger("Roar");
            Context.Movement.Lock();
            _roared = true;
            return;
        }

        GameObject found = FindTargetInFOV();
        if (!_roared && found != null)
        {
            Context.Sight.SetTarget(found);
            Context.Combat.SetFightMode(true);
            Context.Animator.SetTrigger("Roar");
            Context.Movement.Lock();
            _roared = true;
            return;
        }

        if (_alerted)
        {
            Machine.TryActivateState<DragonState_Alert>(true);
        }
    }

    private void TryActiveRandomSubState()
    {
        int rand = Random.Range(0, 2);

        switch (rand)
        {
            case 0:
                _subStateMachine.TryActivateState<DragonState_Wait>(true);
                break;
            case 1:
                _subStateMachine.TryActivateState<DragonState_Patrol>(true);
                break;
            default:
                _subStateMachine.TryActivateState<DragonState_Wait>(true);
                break;
        }
    }

    protected override void CollectChildStateMachines(List<IStateMachine> stateMachines)
    {
        _subStateMachine = new StateMachine<DragonSubStateBase>("DragonIdleSubStateMachine",
            new DragonState_Wait(Context, this),
            new DragonState_Patrol(Context, this));

        stateMachines.Add(_subStateMachine);
    }

    public void OnSubStateComplete()
    {
        TryActiveRandomSubState();
    }

    private GameObject FindTargetInFOV()
    {
        foreach (var collider in Context.Sight.SightDetector.DetectedColliders)
        {
            return collider.gameObject;
        }

        return null;
    }

    public void OnExitMoment()
    {
        Context.Movement.Unlock();
        _alerted = true;
    }
}