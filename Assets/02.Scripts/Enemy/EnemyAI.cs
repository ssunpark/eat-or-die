using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Fusion.Addons.FSM;
using RaycastPro.Detectors;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachineController))]
public class EnemyAI : NetworkBehaviour, IStateMachineOwner, IMoveable, IDetector, IAttackable
{
	[SerializeField] private int _enemyId; // 몬스터 ID

	[Networked] public EAnimationState AnimationState { get; set; }
	
	public NetworkObject NetworkObject => Object;

	private Animator _animator;

	private ChangeDetector _changeDetector;
	
	public EnemyStatManager EnemyStatManager;

	private RangeDetector _rangeDetector;

	[SerializeField] private float _currentHunger;
	private float _takenDamage = 0f;
	
	public EnemyContext Context { get; private set; }

	[SerializeField] private SpawnBehaviour _spawnBehaviour;
	[SerializeField] private IdleBehaviour _idleBehaviour;
	[SerializeField] private MoveBehaviour _moveBehaviour;
	[SerializeField] private AttackBehaviour _attackBehaviour;
	[SerializeField] private HitBehaviour _hitBehaviour;
	[SerializeField] private DieBehaviour _dieBehaviour;

	private bool _hit = false;

	private EnemyBehaviourMachine _behaviourMachine;

	public override void Spawned()
	{
		_rangeDetector = GetComponent<RangeDetector>();
		_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
		
		Context.Agent.updatePosition = false;
		Context.Agent.updateRotation = false;
	}

	public void CollectStateMachines(List<IStateMachine> stateMachines)
	{
		EnemyStatManager = new EnemyStatManager(_enemyId);

		_currentHunger = EnemyStatManager.GetStat(EStatType.EnemyHunger);
		_animator = GetComponent<Animator>();
		
		Context = new EnemyContext()
		{
			Owner = this,
			Target = null,
			Animator = _animator,
			StatManager = EnemyStatManager,
			AnimationRelay = GetComponent<EnemyAnimationRelay>(),
			Agent = GetComponent<NavMeshAgent>(),
			Mover = this,
			Detector = this,
		};
		
		AEnemyStateBehaviour[] stateList = new AEnemyStateBehaviour[]
		{
			_spawnBehaviour,
			_idleBehaviour,
			_moveBehaviour,
			_attackBehaviour,
			_hitBehaviour,
			_dieBehaviour,
		};
		
		_behaviourMachine = new EnemyBehaviourMachine("Behaviour Machine", Context, stateList);
		
		Context.Agent.speed = Context.StatManager.GetStat(EStatType.EnemyMoveSpeed);
		Context.AnimationRelay.SetMachine(_behaviourMachine);
		
		stateMachines.Add(_behaviourMachine);
	}

	public override void FixedUpdateNetwork()
	{
		if (_behaviourMachine.ActiveState is DieBehaviour) return;
		
		if (_hit)
		{
			_currentHunger -= _takenDamage;
			if (_currentHunger <= 0)
			{
				_behaviourMachine.ForceActivateState<DieBehaviour>();
				return;
			}
			_behaviourMachine.ForceActivateState<HitBehaviour>();
			_hit = false;
			_takenDamage = 0f;
		}
		
		if (_rangeDetector.Cast())
		{
			Detect();
		}
	}
	
	public override void Render()
	{
		foreach (string change in _changeDetector.DetectChanges(this))
		{
			switch (change)
			{
				case nameof(AnimationState):
					PlayAnimation();
					break;
			}
		}
	}

	private void PlayAnimation()
	{
		if (_animator == null) return;
        
		string stateName = AnimationState.ToString();
		_animator.CrossFade(stateName, 0.1f);
	}

	public void Move()
	{
		if (Context.Agent.pathPending || !Context.Agent.hasPath) return;
		
		Vector3 direction = Context.Agent.nextPosition - transform.position;
		transform.forward = direction.normalized;
		
		if (direction.sqrMagnitude < 0.01f) return;
		
		direction.Normalize();

		transform.position += direction * Context.Agent.speed * Runner.DeltaTime;
	}

	public void Detect()
	{
		Context.Target = _rangeDetector.NearestMember.gameObject;
		
		float distance = Vector3.Distance(transform.position, Context.Target.transform.position);
		
		if (distance <= _rangeDetector.Radius)
		{
			_behaviourMachine.TryActivateState<MoveBehaviour>();
		}
	}

	public void OnHitLocal(AttackInfo attack)
	{
		if (HasStateAuthority)
		{
			float amount = (attack.MeleeDamage + attack.MagicDamage) * attack.TotalDamageMultiplier;
			float defense = EnemyStatManager.GetStat(EStatType.EnemyMeleeDefense);
			_takenDamage += amount * (100 / (100 + defense));
		
			_hit = true;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public void RPC_HitByAttack(AttackInfo attack)
	{
	}

	public void OnHitStateAuthority(AttackInfo attack)
	{
	}
}
