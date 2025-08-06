using System;
using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class DragonState_Idle : DragonStateBase, IParentState
{
    private StateMachine<DragonSubStateBase> _subStateMachine;
    private DragonStateParameterSet.BaseParams _baseParams;

    public DragonState_Idle(DragonController controller, DragonParameterLoader paramLoader) : base(controller, paramLoader)
    {
        _baseParams = paramLoader.Base;
    }

    protected override void OnEnterState()
    {
        Controller.SetNavMeshAgentMoveData(_baseParams.MoveSpeed, _baseParams.RotationSpeed);
        
        Controller.FightMode(false);
        
        Controller.SetSightDetector(_baseParams.FullAwarenessRadius, _baseParams.DetectRadius, _baseParams.FOVAngle);

        TryActiveRandomSubState();
    }

    protected override void OnFixedUpdate()
    {
        if (Controller.IsLocked) return;

        // 이미 타겟 있으면 Alert 전환
        if (Controller.Target != null)
        {
            Machine.TryActivateState<DragonState_Alert>(true);
            return;
        }

        GameObject found = FindTargetInFOV();
        if (found != null)
        {
            Controller.SetTarget(found);
            Machine.TryActivateState<DragonState_Alert>(true);
        }
    }

    protected override void OnExitState()
    {
        Controller.FightMode(true);
        Controller.Animator.SetTrigger("Roar");
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
            new DragonState_Wait(Controller, this, ParameterLoader.Wait), 
            new DragonState_Patrol(Controller, this, ParameterLoader.Patrol));

        stateMachines.Add(_subStateMachine);
    }

    public void OnSubStateComplete()
    {
        TryActiveRandomSubState();
    }
    
    private GameObject FindTargetInFOV()
    {
        foreach (var collider in Controller.SightDetector.DetectedColliders)
        {
            return collider.gameObject;
        }
        return null;
    }
}