using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Fusion.Addons.FSM;
using RaycastPro.Detectors;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachineController))]
public class EnemyAI : NetworkBehaviour, IStateMachineOwner, IMoveable, IDetector, IAttackable
{
	public int EnemyID; // 몬스터 ID

	[Networked] public EAnimationState AnimationState { get; set; }
	private bool _hit;
	
	public NetworkObject NetworkObject => Object;

	private Animator _animator;

	private ChangeDetector _changeDetector;
	
	public EnemyStatManager EnemyStatManager;

	private RangeDetector _raycastComponent;

	[SerializeField] private float _currentHunger;
	
	public EnemyContext Context { get; private set; }

	[SerializeField] private SpawnBehaviour _spawnBehaviour;
	[SerializeField] private IdleBehaviour _idleBehaviour;
	[SerializeField] private MoveBehaviour _moveBehaviour;
	[SerializeField] private AttackBehaviour _attackBehaviour;
	[SerializeField] private HitBehaviour _hitBehaviour;
	[SerializeField] private DieBehaviour _dieBehaviour;


	private EnemyBehaviourMachine _behaviourMachine;

	public override void Spawned()
	{
		_raycastComponent = GetComponent<RangeDetector>();
		_raycastComponent.Radius = Context.StatManager.GetStat(EStatType.EnemyDetectionRange);
		_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
		
		Context.Agent.updatePosition = false;
		Context.Agent.updateRotation = false;
		Context.Agent.updateUpAxis = false;
	}

	public void CollectStateMachines(List<IStateMachine> stateMachines)
	{
		EnemyStatManager = new EnemyStatManager(EnemyID);

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
			RaycastComponent = GetComponent<RangeDetector>(),
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
			if (_currentHunger <= 0)
			{
				_behaviourMachine.ForceActivateState<DieBehaviour>();
				return;
			}
			_behaviourMachine.ForceActivateState<HitBehaviour>();
			_hit = false;
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

	public bool Detect()
	{
		Collider closestTarget = _raycastComponent.NearestMember;
		
		float distance = Vector3.Distance(transform.position, closestTarget.transform.position);
		
		if (distance <= _raycastComponent.Radius)
		{
			Context.Target = closestTarget.GetComponent<Player>();
			_behaviourMachine.TryActivateState<MoveBehaviour>();
			return true;
		}

		return false;
	}

	public void OnHitLocal(AttackInfo attack)
	{
		if (HasStateAuthority)
		{
			float meleeAmount = attack.MeleeDamage * attack.TotalDamageMultiplier;
			float magicAmount = attack.MagicDamage * attack.TotalDamageMultiplier;
			float meleeDefense = EnemyStatManager.GetStat(EStatType.EnemyMeleeDefense);
			float magicDefense = EnemyStatManager.GetStat(EStatType.EnemyMagicDefense);
			
			float totalDamage = meleeAmount * (100 / (100 + meleeDefense)) + magicAmount * (100 / (100 + magicDefense));

			_currentHunger -= totalDamage;
		
			_hit = true;
			
			Context.Target = attack.Attacker.GetComponent<Player>();
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
