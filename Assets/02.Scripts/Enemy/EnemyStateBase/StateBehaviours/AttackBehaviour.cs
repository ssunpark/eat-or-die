using UnityEngine;
using Fusion.Addons.FSM;

public class AttackBehaviour : AEnemyStateBehaviour, IEventReceiver
{
	private static readonly int Attack = Animator.StringToHash("Attack");
	private bool _isAttackFinished = false;
	
	protected override void OnEnterState()
	{
		Debug.Log("Attacking...");
		_isAttackFinished = false;
		Machine.Context.Animator.SetTrigger(Attack);
		Machine.Context.AnimationRelay.SetReceiver(this);
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
		Machine.Context.AnimationRelay.RemoveReceiver();
	}

	protected override bool CanExitState(AEnemyStateBehaviour nextState)
	{
		return _isAttackFinished;
	}

	protected override void OnEnterStateRender()
	{
	}

	public void OnActionMoment()
	{
		if (!HasStateAuthority) return;
		
		Debug.Log("Attack Moment Triggered");
	}
}
