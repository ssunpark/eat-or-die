using Fusion.Addons.FSM;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateBehaviour : StateBehaviour<EnemyStateBehaviour>
{
	protected Animator Animator;
	protected NavMeshAgent Agent;

	protected override void OnInitialize()
	{
		Animator = GetComponentInParent<Animator>();
		Agent = GetComponentInParent<NavMeshAgent>();
	}
}