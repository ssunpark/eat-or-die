using System;
using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

[Serializable]
public class DragonState_Alert : DragonStateBase, IParentState
{
    private StateMachine<DragonSubStateBase> _subStateMachine;

    public DragonState_Alert(DragonController controller) : base(controller)
    {
    }

    protected override void OnEnterState()
    {
        Debug.Log("Alert 상태 진입");

        Controller.FightMode(true);
        Controller.Lock();
        Controller.OnUnlock += OnUnlock;
        Controller.Animator.SetTrigger("Roar");
    }

    private void OnUnlock()
    {
        _subStateMachine.TryActivateState<DragonState_Look>(true);
        Controller.OnUnlock -= OnUnlock;
    }

    protected override void OnFixedUpdate()
    {
        if (Controller.IsLocked)
        {
            return; // 잠금 상태면 아무 것도 안 함
        }

        if (Controller.Target == null)
        {
            Machine.TryActivateState<DragonState_Idle>(true);
        }
    }

    protected override void CollectChildStateMachines(List<IStateMachine> stateMachines)
    {
        _subStateMachine = new StateMachine<DragonSubStateBase>("DragonAlertSubStateMachine", new DragonState_Look(Controller, this));
        
        stateMachines.Add(_subStateMachine);
    }

    public void OnSubStateComplete()
    {
        _subStateMachine.TryActivateState<DragonState_Look>(true);
    }
}