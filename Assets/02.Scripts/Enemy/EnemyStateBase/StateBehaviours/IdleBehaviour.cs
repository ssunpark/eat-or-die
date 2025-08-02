using System;
using Fusion.Addons.FSM;
using UnityEngine;

public class IdleBehaviour : AEnemyStateBehaviour
{
	private static readonly int Idle = Animator.StringToHash("Idle");
	
	protected override void OnEnterState()
	{
		Debug.Log("Idling...");
		Machine.Context.Animator.SetTrigger(Idle);
	}
	
	protected override void OnExitState()
	{
	}
	
	protected override void OnEnterStateRender()
	{
	}

	protected override bool CanExitState(AEnemyStateBehaviour nextStateBehaviour)
	{
		return Machine.StateTime >= 0.1f;
	}
}
