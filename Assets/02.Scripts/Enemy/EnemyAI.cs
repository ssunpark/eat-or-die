using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Fusion.Addons.FSM;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachineController))]
public class EnemyAI : NetworkBehaviour, IStateMachineOwner, IMoveable
{
	[SerializeField] private int _enemyId;
	public EnemyContext Context { get; private set; }
	
	[SerializeField] private IdleBehaviour _idleBehaviour;

	private EnemyBehaviourMachine _behaviourMachine;

	public override void Spawned()
	{
		Context = new EnemyContext
		{
			Target = null,
			Stat = new EnemyStat(),
			Animator = GetComponent<Animator>(),
			Agent = GetComponent<NavMeshAgent>(),
			Mover = this,
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
	}

	public void Move()
	{
		Vector3 desiredVelocity = Context.Agent.desiredVelocity;

		if (desiredVelocity.sqrMagnitude < 0.01f) return;
		
		Vector3 direction = desiredVelocity.normalized;
		
		transform.forward = direction;
		transform.position += direction * Context.Stat.MoveSpeed * Runner.DeltaTime;
	}
}
