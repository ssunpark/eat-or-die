using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;

public class AttackBehaviour : AEnemyStateBehaviour, IEventReceiver, IParticlePlayer
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
		
		return distance <= Machine.Context.StatManager.GetStat(EStatType.EnemyTriggerRange)
		       && Vector3.Angle(toTarget, transform.forward) <= Machine.Context.StatManager.GetStat(EStatType.EnemyTriggerAngle);
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
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, Machine.Context.StatManager.GetStat(EStatType.EnemyAttackRange), _attackableLayer);
		foreach (Collider targetCollider in hitColliders)
		{
			Vector3 toTarget = targetCollider.transform.position - transform.position;
			if (Vector3.Angle(toTarget, transform.forward) > Machine.Context.StatManager.GetStat(EStatType.EnemyAttackAngle)) continue;
				    
			if (targetCollider.TryGetComponent(out IAttackable attackable))
			{
				AttackInfo attackInfo = new AttackInfo
				{
					MeleeDamage = Machine.Context.StatManager.GetStat(EStatType.EnemyDamage),
					Attacker = Machine.Context.Owner.Object,
					TotalDamageMultiplier = 1f / Machine.Context.StatManager.GetStat(EStatType.EnemyHitCount),
				};
				attackable.OnHitLocal(attackInfo);
			}
		}
		Debug.Log("Attack Moment Triggered");
	}

	public void PlayParticle()
	{
		int id = Machine.Context.Owner.EnemyID;

		string particleKey = EnemyDataManager.Instance.EnemyRawDataDictionary[id].AttackParticleKey;
		
		if (string.IsNullOrEmpty(particleKey)) return;
		
		ParticleManager.Instance.PlayByKey(particleKey, transform.position, transform.rotation, false);
	}
}
