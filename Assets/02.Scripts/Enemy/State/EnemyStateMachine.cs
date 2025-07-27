using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : NetworkBehaviour
{
	// private EnemyStat _stat;
	private ChangeDetector _changeDetector;

	private GameObject _target;
	public GameObject Target => _target;

	private Animator _animator;
	public Animator Animator => _animator;
	
	private NavMeshAgent _navMeshAgent;
	public NavMeshAgent NavMeshAgent => _navMeshAgent;
	
	// private CharacterController _characterController;
	
	[Networked] private EEnemyState NetworkedState { get; set; }
	
	private IEnemyState<EnemyStateMachine> _currentState;
	private Dictionary<EEnemyState, IEnemyState<EnemyStateMachine>> _stateDictionary;
	
	// Stat으로 분리시킬 가능성 높음

	private float _moveSpeed = 5f;
	private float _attackRange = 2f;
	public float AttackRange => _attackRange;
	
	public override void Spawned()
	{
		_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
		_animator = GetComponent<Animator>();
		_navMeshAgent = GetComponent<NavMeshAgent>();
		// _characterController = GetComponent<CharacterController>();
		
		_navMeshAgent.updateRotation = false;
		_navMeshAgent.updateUpAxis = false;
		_navMeshAgent.updatePosition = false;
		
		// Stat = GetComponent<EnemyStat>();
		
		_stateDictionary = new Dictionary<EEnemyState, IEnemyState<EnemyStateMachine>>
		{
			{ EEnemyState.Idle, new EnemyIdleState() },
			{ EEnemyState.Trace, new EnemyTraceState() },
			{ EEnemyState.Attack, new EnemyAttackState() },
			// { EEnemyState.Patrol, new EnemyPatrolState() },
			// { EEnemyState.Die, new EnemyDieState() }
		};

		NetworkedState = EEnemyState.Idle;
		_currentState = _stateDictionary[NetworkedState];
	}

	public void SetTarget(GameObject target)
	{
		_target = target;
	}
	
	public override void FixedUpdateNetwork()
	{
		if (HasStateAuthority)
		{
			_currentState?.Update(this, Time.deltaTime);
		}
		foreach (string change in _changeDetector.DetectChanges(this))
		{ 
			if (change == nameof(NetworkedState)) 
			{
				_currentState?.Exit(this);
				_currentState = _stateDictionary[NetworkedState];
				_currentState?.Enter(this);
			}
		}
	}

	public void RequestStateChange(EEnemyState newState)
	{
		if (Object.HasStateAuthority)
		{
			SetState(newState);
		}
		else
		{
			RPC_RequestStateChange(newState);
		}
	}
	
	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	private void RPC_RequestStateChange(EEnemyState newState)
	{
		SetState(newState);
	}

	private void SetState(EEnemyState newState)
	{
		NetworkedState = newState;
	}

	public void Move(Vector3 direction)
	{
		if (!HasStateAuthority) return;
		
		if (direction.magnitude < 0.1f) return;
		
		direction = direction.normalized;
		
		gameObject.transform.forward = direction;
		transform.position += direction * Runner.DeltaTime * _moveSpeed;
	}
}
