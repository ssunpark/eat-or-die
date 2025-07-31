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
	
	[SerializeField] private IdleBehaviour _idleBehaviour;
	[SerializeField] private MoveBehaviour _moveBehaviour;
	[SerializeField] private AttackBehaviour _attackBehaviour;

	private EnemyBehaviourMachine _behaviourMachine;

	public override void Spawned()
	{
		_rangeDetector = GetComponent<RangeDetector>();
		
		Context = new EnemyContext()
		{
			Target = null,
			Stat = new EnemyStat(),
			Animator = GetComponent<Animator>(),
			Agent = GetComponent<NavMeshAgent>(),
			Mover = this,
			Detector = this,
		};
		
		Context.Agent.updatePosition = false;
		Context.Agent.updateRotation = false;
	}
	
	public void CollectStateMachines(List<IStateMachine> stateMachines)
	{
		_behaviourMachine = new EnemyBehaviourMachine("Behaviour Machine", Context, _idleBehaviour);
		
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
		Vector3 desiredVelocity = Context.Agent.desiredVelocity;

		if (desiredVelocity.sqrMagnitude < 0.01f) return;
		
		Vector3 direction = desiredVelocity.normalized;
		
		transform.forward = direction;
		transform.position += direction * Context.Stat.MoveSpeed * Runner.DeltaTime;
	}

	public void Detect()
	{
		Context.Target = _rangeDetector.NearestMember.gameObject;
		
		float distance = Vector3.Distance(transform.position, Context.Target.transform.position);
		
		Debug.Log($"Target detected at distance {distance}");
		if (distance <= Context.Stat.AttackRange)
		{
			_behaviourMachine.TryActivateState(_attackBehaviour);
		}
		else
		{
			_behaviourMachine.TryActivateState(_moveBehaviour);
		}
	}
}
