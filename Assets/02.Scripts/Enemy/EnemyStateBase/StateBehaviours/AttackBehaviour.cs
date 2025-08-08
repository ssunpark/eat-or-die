using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;

public class AttackBehaviour : AEnemyStateBehaviour, IEventReceiver
{
	private EnemyStateMachine _attackStateMachine;
	
	public bool IsAttackFinished = false;

	private AttackPrepareState _prepareState = new AttackPrepareState();
	private AttackActionState _actionState = new AttackActionState();
	
	protected override void OnCollectChildStateMachines(List<IStateMachine> stateMachines)
	{
		AEnemyState[] stateList = new AEnemyState[]
		{
			_prepareState,
			_actionState,
		};
		_attackStateMachine = new EnemyStateMachine("Attack State Machine", this, stateList);
        
		stateMachines.Add(_attackStateMachine);
	}

	protected override bool CanEnterState()
	{
		Vector3 toTarget = Machine.Context.Target.transform.position - transform.position;
		float distance = toTarget.magnitude;
		
		return distance <= Machine.Context.StatManager.GetStat(EStatType.EnemyAttackRange)
		       && Vector3.Angle(toTarget, transform.forward) <= Machine.Context.StatManager.GetStat(EStatType.EnemyAttackAngle);
	}
	
	protected override void OnEnterState()
	{
		_attackStateMachine.TryActivateState(_prepareState);
		
		// _isAttackFinished = false;
		// Machine.Context.Owner.AnimationState = EAnimationState.Attack;
	}

	protected override void OnFixedUpdate()
	{

	}

	protected override void OnExitState()
	{
		IsAttackFinished = false;
	}

	protected override bool CanExitState(AEnemyStateBehaviour nextState)
	{
		return IsAttackFinished;
	}

	public void OnActionMoment()
	{
		Debug.Log("Attack Moment Triggered");
	}
}
