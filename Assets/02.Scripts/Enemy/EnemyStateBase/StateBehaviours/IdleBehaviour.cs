using System;
using Fusion.Addons.FSM;
using UnityEngine;

public class IdleBehaviour : AEnemyStateBehaviour
{
	protected override void OnEnterState()
	{
		Debug.Log("Idling...");
		Machine.Context.Owner.AnimationState = EAnimationState.Idle;
	}
}	
