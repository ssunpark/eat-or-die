using UnityEngine;

public class DragonMeleeAttack_Prepare : DragonSubStateBase
{
    private DragonStateParameterSet.PrepareParams _prepareParams;
    public DragonMeleeAttack_Prepare(
        DragonController controller,
        IParentState parent,
        DragonStateParameterSet.PrepareParams prepareParams)
        : base(controller, parent)
    {
        _prepareParams = prepareParams;
    }

    protected override bool CanEnterState()
    {
        if (Controller.IsLocked)
        {
            return false;
        }
        
        float distance = Vector3.Distance(
            Controller.transform.position,
            Controller.Target.transform.position
        );
        float prepareRandom = Random.Range(0f, 1f);
        return distance < _prepareParams.MinDistanceToFinishPrepare && 
               prepareRandom < _prepareParams.PrepareChance;
    }

    protected override void OnEnterState()
    {
        Controller.NavMeshAgent.enabled = false;
        Controller.Animator.SetBool("IsMove", false);
        Controller.Animator.SetBool("IsBackStep", true); // 회전 중에는 움직이는 듯한 연출
    }

    protected override void OnFixedUpdate()
    {
        if (Controller.IsLocked)
        {
            return;
        }
        
        Controller.MaintainDistanceAndLookAtTarget(Machine.Runner.DeltaTime, _prepareParams.MinDistanceToFinishPrepare);

        // 거리 측정
        float distanceToTarget = Vector3.Distance(
            Controller.transform.position,
            Controller.Target?.transform.position ?? Controller.transform.position
        );

        // 시간 또는 거리 조건 만족 시 종료
        if (Machine.StateTime >= _prepareParams.PrepareDuration ||
            distanceToTarget >= _prepareParams.MinDistanceToFinishPrepare)
        {
            ParentState.OnSubStateComplete();
        }
    }

    protected override void OnExitState()
    {
        Controller.NavMeshAgent.enabled = true;
        Controller.Animator.SetBool("IsMove", true);
        Controller.Animator.SetBool("IsBackStep", false);
    }
}