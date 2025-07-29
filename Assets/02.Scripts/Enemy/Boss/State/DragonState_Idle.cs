using System;
using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class DragonState_Idle : DragonStateBase, IParentState
{
    private StateMachine<DragonSubStateBase> _subStateMachine;

    public DragonState_Idle(DragonController controller) : base(controller)
    {
    }

    protected override void OnEnterState()
    {
        Debug.Log("Idle 상태 진입");

        Controller.FightMode(false);

        if (!Controller.IsLocked)
        {
            TryActiveRandomSubState();
        }
        else
        {
            Controller.OnUnlock += OnUnlock;
        }
    }

    private void OnUnlock()
    {
        TryActiveRandomSubState();
        Controller.OnUnlock -= OnUnlock;
    }

    protected override void OnFixedUpdate()
    {
        if (Controller.IsLocked)
        {
            return; // 잠금 상태면 아무 것도 안 함
        }
        
        if (Controller.Target != null)
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
        _subStateMachine = new StateMachine<DragonSubStateBase>("DragonIdleSubStateMachine", new DragonState_Wait(Controller, this), new DragonState_Patrol(Controller, this));
        
        stateMachines.Add(_subStateMachine);
    }

    public void OnSubStateComplete()
    {
        TryActiveRandomSubState();
    }
}