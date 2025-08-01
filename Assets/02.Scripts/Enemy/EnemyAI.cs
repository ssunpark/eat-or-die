using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Fusion.Addons.FSM;
using RaycastPro.Detectors;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachineController))]
public class EnemyAI : NetworkBehaviour, IStateMachineOwner, IMoveable, IDetector
{
	[SerializeField] private int _enemyId;
	
	private RangeDetector _rangeDetector;
	
	public EnemyContext Context { get; private set; }
	
	[SerializeField] private SpawnBehaviour _spawnBehaviour;
	[SerializeField] private IdleBehaviour _idleBehaviour;
	[SerializeField] private MoveBehaviour _moveBehaviour;
	[SerializeField] private AttackBehaviour _attackBehaviour;

	private EnemyBehaviourMachine _behaviourMachine;

	public override void Spawned()
	{
		_rangeDetector = GetComponent<RangeDetector>();
		
		Context.Agent.updatePosition = false;
		Context.Agent.updateRotation = false;
	}
	
	public void CollectStateMachines(List<IStateMachine> stateMachines)
	{
		Context = new EnemyContext()
		{
			Target = null,
			Stat = new EnemyStat(),
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
			_attackBehaviour
		};
		
		_behaviourMachine = new EnemyBehaviourMachine("Behaviour Machine", Context, stateList);
		
		stateMachines.Add(_behaviourMachine);
	}

	public override void FixedUpdateNetwork()
	{
		if (_rangeDetector.Cast())
		{
			Detect();
		}
	}

	public void Move()
	{
		Vector3 direction = Context.Agent.nextPosition - transform.position;
		transform.forward = direction;
		
		if (direction.sqrMagnitude < 0.01f) return;
		
		direction.Normalize();
		
		transform.position += direction * Context.Stat.MoveSpeed * Runner.DeltaTime;
	}

	public void Detect()
	{
		Context.Target = _rangeDetector.NearestMember.gameObject;
		
		float distance = Vector3.Distance(transform.position, Context.Target.transform.position);
		float angle = Vector3.Angle(transform.forward, Context.Target.transform.position - transform.position);
		
		if (distance <= Context.Stat.AttackRange && angle <= Context.Stat.AttackAngle)
		{
			_behaviourMachine.TryActivateState<AttackBehaviour>();
		}
		else
		{
			_behaviourMachine.TryActivateState<MoveBehaviour>();
		}
	}
}
