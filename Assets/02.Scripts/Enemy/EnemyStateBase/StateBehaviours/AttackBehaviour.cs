using UnityEngine;
using Fusion.Addons.FSM;

public class AttackBehaviour : AEnemyStateBehaviour, IEventReceiver
{
	private bool _isAttackFinished = false;

	protected override bool CanEnterState()
	{
		if (Machine.Context.Animator.IsInTransition(0) ||
		    Machine.StateTime < Machine.Context.StatManager.GetStat(EStatType.EnemyAttackSpeed))
		{
			return false;
		}
		
		Vector3 toTarget = Machine.Context.Target.transform.position - transform.position;
		float distance = toTarget.magnitude;
		
		return distance <= Machine.Context.StatManager.GetStat(EStatType.EnemyAttackRange)
		       && Vector3.Angle(toTarget, transform.forward) <= Machine.Context.StatManager.GetStat(EStatType.EnemyAttackAngle);
	}
	
	protected override void OnEnterState()
	{
		_isAttackFinished = false;
		Machine.Context.Owner.AnimationState = EAnimationState.Attack;
	}

	protected override void OnFixedUpdate()
	{
		AnimatorStateInfo stateInfo = Machine.Context.Animator.GetCurrentAnimatorStateInfo(0);

		if (!Machine.Context.Animator.IsInTransition(0) && stateInfo.normalizedTime >= 1f)
		{
			_isAttackFinished = true;
			Machine.TryActivateState<IdleBehaviour>();
		}
	}

	protected override void OnExitState()
	{
		_isAttackFinished = false;
	}

	protected override bool CanExitState(AEnemyStateBehaviour nextState)
	{
		return _isAttackFinished;
	}

	public void OnActionMoment()
	{
		Debug.Log("Attack Moment Triggered");
	}
}
