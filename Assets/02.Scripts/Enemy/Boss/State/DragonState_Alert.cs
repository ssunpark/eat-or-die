using System;
using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

[Serializable]
public class DragonState_Alert : DragonStateBase, IParentStateMachine
{
    private StateMachine<DragonSubStateBase> _subStateMachine;

    public DragonState_Alert(DragonStateMachine machine) : base(machine)
    {
    }

    protected override void OnEnterState()
    {
        Debug.Log("Alert 상태 진입");

        StateMachine.FightMode(true);
        StateMachine.Lock();
        StateMachine.OnUnlock += OnUnlock;
        StateMachine.Animator.SetTrigger("Roar");
    }

    private void OnUnlock()
    {
        _subStateMachine.TryActivateState<DragonState_Look>(true);
        StateMachine.OnUnlock -= OnUnlock;
    }

    protected override void OnFixedUpdate()
    {
        if (StateMachine.IsLocked)
        {
            return; // 잠금 상태면 아무 것도 안 함
        }

        if (StateMachine.Target == null)
        {
            Machine.TryActivateState<DragonState_Idle>(true);
        }
    }

    protected override void CollectChildStateMachines(List<IStateMachine> stateMachines)
    {
        _subStateMachine = new StateMachine<DragonSubStateBase>("DragonAlertSubStateMachine", new DragonState_Look(StateMachine, this));
        
        stateMachines.Add(_subStateMachine);
    }

    public void OnSubStateComplete()
    {
        _subStateMachine.TryActivateState<DragonState_Look>(true);
    }
}