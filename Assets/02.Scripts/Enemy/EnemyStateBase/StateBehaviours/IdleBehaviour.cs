using System;
using Fusion.Addons.FSM;
using UnityEngine;

public class IdleBehaviour : AEnemyStateBehaviour
{
	protected override void OnEnterStateRender()
	{
		Debug.Log("Idling...");
	}

	protected override bool CanEnterState()
	{
		return base.CanEnterState();
	}

	protected override bool CanExitState(AEnemyStateBehaviour nextState)
	{
		return base.CanExitState(nextState);
	}
}
