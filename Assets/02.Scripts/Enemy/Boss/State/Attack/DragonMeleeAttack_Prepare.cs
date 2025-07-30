// using UnityEngine;
//
// public class DragonMeleeAttack_Prepare : DragonSubStateBase
// {
//     private DragonStateParameterSet.PrepareParams _prepareParams;
//     public DragonMeleeAttack_Prepare(
//         DragonController controller,
//         IParentState parent,
//         DragonStateParameterSet.PrepareParams prepareParams)
//         : base(controller, parent)
//     {
//         _prepareParams = prepareParams;
//     }
//
//     protected override void OnEnterState()
//     {
//         Controller.NavMeshAgent.enabled = false;
//         Controller.Animator.SetBool("IsMove", false);
//         Controller.Animator.SetBool("IsBackStep", true); // 회전 중에는 움직이는 듯한 연출
//     }
//
//     protected override void OnFixedUpdate()
//     {
//         Controller.MaintainDistanceAndLookAtTarget(Machine.Runner.DeltaTime, 10f);
//
//         if (Machine.StateTime >= _prepareParams.PrepareDuration)
//         {
//             ParentState.OnSubStateComplete();
//         }
//     }
//
//     protected override void OnExitState()
//     {
//         Controller.NavMeshAgent.enabled = true;
//         Controller.Animator.SetBool("IsBackStep", false);
//     }
// }