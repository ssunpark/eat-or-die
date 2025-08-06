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

	public EnemyStatManager EnemyStatManager;

	public int HitCountTemp = 0;
	
	private RangeDetector _rangeDetector;
	
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
		
		Context.Agent.updatePosition = false;
		Context.Agent.updateRotation = false;
	}
	
	public void CollectStateMachines(List<IStateMachine> stateMachines)
	{
		EnemyStatManager = new EnemyStatManager(_enemyId);
		
		Context = new EnemyContext()
		{
			Target = null,
			StatManager = EnemyStatManager,
			Animator = GetComponent<Animator>(),
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
		
		stateMachines.Add(_behaviourMachine);
	}

	public override void FixedUpdateNetwork()
	{
		if (_behaviourMachine.ActiveState is DieBehaviour) return;
		
		if (_hit)
		{
			HitCountTemp++;
			if (HitCountTemp >= 3)
			{
				_behaviourMachine.ForceActivateState<DieBehaviour>();
				return;
			}
			_behaviourMachine.ForceActivateState<HitBehaviour>();
			_hit = false;
		}
		
		if (_rangeDetector.Cast())
		{
			Detect();
		}
	}

	public void Move()
	{
		if (!Context.Agent.hasPath) return;
		
		Vector3 direction = Context.Agent.nextPosition - transform.position;
		transform.forward = direction;
		
		if (direction.sqrMagnitude < 0.01f) return;
		
		direction.Normalize();
		
		transform.position += direction * Context.StatManager.GetStat(EStatType.EnemyMoveSpeed) * Runner.DeltaTime;
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

	// IAttackable Interface Implementation
	public NetworkObject NetworkObject { get; }
	
	public void OnHitLocal(AttackInfo attack, NetworkObject attacker)
	{
		if (HasStateAuthority)
		{
			_hit = true;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public void RPC_HitByAttack(AttackInfo attack, NetworkObject attacker)
	{
	}

	public void OnHitStateAuthority(AttackInfo attack, NetworkObject attacker)
	{
	}
}
