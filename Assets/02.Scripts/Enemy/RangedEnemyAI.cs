using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Fusion.Addons.FSM;

[RequireComponent(typeof(StateMachineController))]
public class RangedEnemyAI : NetworkBehaviour, IStateMachineOwner
{
	[SerializeField] private IdleBehaviour _idleState;
	[SerializeField] private AttackBehaviour _attackState;

	private StateMachine<StateBehaviour> _rangedEnemyAI;
	
	public void CollectStateMachines(List<IStateMachine> stateMachines)
	{
		_rangedEnemyAI = new StateMachine<StateBehaviour>("Ranged Enemy AI", _idleState, _attackState);
		
		stateMachines.Add(_rangedEnemyAI);
	}

	public override void FixedUpdateNetwork()
	{
		_rangedEnemyAI.TryActivateState(_attackState);
	}
}
