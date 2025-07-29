using Fusion.Addons.FSM;
using UnityEngine;

public class IdleBehaviour : StateBehaviour
{
	protected override bool CanExitState(StateBehaviour nextState)
	{
		// Wait at least 3 seconds before idling finishes
		return Machine.StateTime > 3f;
	}

	protected override void OnEnterStateRender()
	{
		Debug.Log("Idling...");
	}	
}
