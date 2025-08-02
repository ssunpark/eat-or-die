using UnityEngine;
using Fusion.Addons.FSM;

public class AttackBehaviour : AEnemyStateBehaviour
{
	private static readonly int Attack = Animator.StringToHash("Attack");
	private bool _isAttackFinished = false;
	
	protected override void OnEnterState()
	{
		Debug.Log("Attacking...");
		Machine.Context.Animator.SetTrigger(Attack);
	}

	protected override void OnFixedUpdate()
	{
		AnimatorStateInfo stateInfo = Machine.Context.Animator.GetCurrentAnimatorStateInfo(0);

		if (!Machine.Context.Animator.IsInTransition(0) && stateInfo.normalizedTime >= 1f)
		{
			_isAttackFinished = true;
			Machine.TryActivateState<MoveBehaviour>();
		}
	}

	protected override void OnExitState()
	{
		_isAttackFinished = false;
		Machine.Context.Animator.ResetTrigger(Attack);
	}

	protected override bool CanExitState(AEnemyStateBehaviour nextState)
	{
		return _isAttackFinished;
	}

	protected override void OnEnterStateRender()
	{
	}	
}
