using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : NetworkBehaviour
{
	// private EnemyStat _stat;
	
	private Animator _animator;
	public Animator Animator => _animator;
	
	private CharacterController _characterController;
	public CharacterController CharacterController => _characterController;
	
	private NavMeshAgent _navMeshAgent;
	public NavMeshAgent NavMeshAgent => _navMeshAgent;
	
	private IEnemyState<EnemyStateMachine> _currentState;
	private Dictionary<EEnemyState, IEnemyState<EnemyStateMachine>> _stateDictionary;
	
	public override void Spawned()
	{
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
	
	private void Update()
	{
		_currentState?.Update(this, Time.deltaTime);
	}
	
	public void RequestStateChange(EEnemyState newState)
	{
	}
	
	public void EventStateChange(EEnemyState newState)
	{
	}
}
