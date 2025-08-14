using UnityEngine;

public class DragonMeleeAttack_Prepare : DragonSubStateBase
{
    private DragonStateParameterSet.PrepareParams _prepareParams;
    public DragonMeleeAttack_Prepare(
        DragonContext context,
        IParentState parent)
        : base(context, parent)
    {
        _prepareParams = Context.Parameter.Prepare;
    }

    protected override bool CanEnterState()
    {
        float distance = Context.Sight.Distance;
        float prepareRandom = Random.Range(0f, 1f);
        return distance < _prepareParams.MinDistanceToFinishPrepare && 
               prepareRandom < _prepareParams.PrepareChance;
    }

    protected override void OnEnterState()
    {
        Context.Movement.NavMeshAgent.enabled = false;
    }

    protected override void OnFixedUpdate()
    {
        Context.Movement.MaintainDistanceAndLookAtTarget(Machine.Runner.DeltaTime, _prepareParams.MinDistanceToFinishPrepare);

        // 거리 측정
        float distanceToTarget = Context.Sight.Distance;

        // 시간 또는 거리 조건 만족 시 종료
        if (Machine.StateTime >= _prepareParams.PrepareDuration ||
            distanceToTarget >= _prepareParams.MinDistanceToFinishPrepare)
        {
            ParentState.OnSubStateComplete();
        }
    }

    protected override void OnExitState()
    {
        Context.Movement.NavMeshAgent.enabled = true;
    }

    protected override void OnEnterStateRender()
    {
        Context.Animator.SetBool("IsMove", false);
        Context.Animator.SetBool("IsBackStep", true); // 회전 중에는 움직이는 듯한 연출
    }

    protected override void OnExitStateRender()
    {
        Context.Animator.SetBool("IsBackStep", false);
    }
}