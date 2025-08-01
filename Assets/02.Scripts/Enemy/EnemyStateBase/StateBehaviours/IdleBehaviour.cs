using System;
using Fusion.Addons.FSM;
using UnityEngine;

public class IdleBehaviour : AEnemyStateBehaviour
{
	private static readonly int Idle = Animator.StringToHash("Idle");
	
	protected override bool CanEnterState()
	{
		return base.CanEnterState();
	}

	protected override bool CanExitState(AEnemyStateBehaviour nextState)
	{
		return base.CanExitState(nextState);
	}
	
	protected override void OnEnterState()
	{
		Machine.Context.Animator.SetBool(Idle, true);
	}
	
	protected override void OnExitState()
	{
		Machine.Context.Animator.SetBool(Idle, false);
	}
	
	protected override void OnEnterStateRender()
	{
		Debug.Log("Idling...");
	}
}
