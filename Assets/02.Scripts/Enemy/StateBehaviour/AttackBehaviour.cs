using UnityEngine;
using Fusion.Addons.FSM;

public class AttackBehaviour : StateBehaviour
{
	protected override void OnFixedUpdate()
	{
		if (Machine.StateTime > 1f)
		{
			// Attack finished, deactivate
			Machine.TryDeactivateState(StateId);
		}
	}

	protected override void OnEnterStateRender()
	{
		Debug.Log("Attacking...");
	}	
}
