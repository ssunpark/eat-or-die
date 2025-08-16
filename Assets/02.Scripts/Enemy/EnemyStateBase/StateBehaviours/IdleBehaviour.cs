using System;
using Fusion.Addons.FSM;
using UnityEngine;

public class IdleBehaviour : AEnemyStateBehaviour
{
	[SerializeField] private bool _isInTransition = false;
	
	protected override void OnEnterState()
	{
		Debug.Log("Idling...");
		_isInTransition = false;
		Machine.Context.Owner.AnimationState = EAnimationState.Idle;
	}

	protected override void OnFixedUpdate()
	{
		if (_isInTransition) return;
		
		if (Machine.Context.RaycastComponent.Cast() && Machine.Context.Detector.Detect())
		{
			_isInTransition = true;
		}
		else if (Machine.Context.Target != null && Machine.Context.Target.PlayerFSM.IsDead == false)
		{
			_isInTransition = true;
			Machine.TryActivateState<MoveBehaviour>();
		}
	}
	
	protected override void OnExitState()
	{
		_isInTransition = false;
	}
}	
