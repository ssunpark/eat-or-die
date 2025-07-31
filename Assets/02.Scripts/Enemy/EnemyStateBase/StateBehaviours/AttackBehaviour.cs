using UnityEngine;
using Fusion.Addons.FSM;

public class AttackBehaviour : AEnemyStateBehaviour
{
	protected override void OnFixedUpdate()
	{
	}

	protected override void OnEnterStateRender()
	{
		Debug.Log("Attacking...");
	}	
}
