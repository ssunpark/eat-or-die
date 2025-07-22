using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : NetworkBehaviour
{
	// private EnemyStat _stat;
	private ChangeDetector _changeDetector;

	private Animator _animator;
	public Animator Animator => _animator;
	
	private CharacterController _characterController;
	public CharacterController CharacterController => _characterController;
	
	private NavMeshAgent _navMeshAgent;
	public NavMeshAgent NavMeshAgent => _navMeshAgent;
	
	[Networked] private EEnemyState NetworkedState { get; set; }
	
	private IEnemyState<EnemyStateMachine> _currentState;
	private Dictionary<EEnemyState, IEnemyState<EnemyStateMachine>> _stateDictionary;
	
	public override void Spawned()
	{
		_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
		_animator = GetComponent<Animator>();
		_characterController = GetComponent<CharacterController>();
		_navMeshAgent = GetComponent<NavMeshAgent>();
		// Stat = GetComponent<EnemyStat>();
		
		_stateDictionary = new Dictionary<EEnemyState, IEnemyState<EnemyStateMachine>>
		{
			// { EEnemyState.Idle, new EnemyIdleState() },
			// { EEnemyState.Patrol, new EnemyPatrolState() },
			// { EEnemyState.Trace, new EnemyTraceState() },
			// { EEnemyState.Attack, new EnemyAttackState() },
			// { EEnemyState.Die, new EnemyDieState() }
		};
		
		_currentState = _stateDictionary[EEnemyState.Idle];
	}
	
	public override void FixedUpdateNetwork()
	{
		if (Object.HasStateAuthority)
		{
			_currentState?.Update(this, Time.deltaTime);
		}
		else
		{
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
}
