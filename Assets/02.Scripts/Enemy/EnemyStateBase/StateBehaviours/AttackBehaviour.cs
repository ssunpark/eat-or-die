using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;
using UnityEditor.Timeline;
using UnityEngine.Analytics;

public class AttackBehaviour : AEnemyStateBehaviour, IEventReceiver
{
	private EnemyStateMachine _attackStateMachine;
	
	public bool IsAttackFinished = false;

	private AttackPrepareState _prepareState = new AttackPrepareState();
	private AttackActionState _actionState = new AttackActionState();
	
	// 공격 감지 레이어
	[SerializeField] private LayerMask _attackableLayer;
	
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
		// 오버랩 스피어로 공격 대상이 범위안에 있는지 탐지
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, Machine.Context.StatManager.GetStat(EStatType.EnemyAttackRange), _attackableLayer);
		foreach (Collider targetCollider in hitColliders)
		{
			// 공격대상이 Attack 각도 밖에 있으면 return
			Vector3 toTarget = targetCollider.transform.position - transform.position;
			if (Vector3.Angle(toTarget, transform.forward) > Machine.Context.StatManager.GetStat(EStatType.EnemyAttackAngle)) continue;
				    
			if (targetCollider.TryGetComponent(out IAttackable attackable))
			{
				AttackInfo attackInfo = new AttackInfo
				{
					MeleeDamage = Machine.Context.StatManager.GetStat(EStatType.EnemyDamage),
					Attacker = Machine.Context.Owner.Object,
					TotalDamageMultiplier = 1,
				};
				attackable.OnHitLocal(attackInfo);
			}
		}
		Debug.Log("Attack Moment Triggered");
	}
}
