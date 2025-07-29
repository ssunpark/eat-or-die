// using UnityEngine;
//
// public class DragonAttackState : IEnemyState<DragonStateMachine>
// {
//     private IEnemyState<DragonStateMachine> _currentAttackSubState;
//     private DragonStateMachine _stateMachine;
//
//     public bool IsInterruptable => false; // 공격 도중 외부 상태 전환 차단
//
//     public void Enter(DragonStateMachine stateMachine)
//     {
//         Debug.Log("Attack 상위 상태 진입");
//
//         _stateMachine = stateMachine;
//
//         // 하위 공격 상태 진입
//         SetAttackSubState(ChooseRandomAttackState());
//     }
//
//     public void Update(DragonStateMachine stateMachine, float deltaTime)
//     {
//         _currentAttackSubState?.Update(stateMachine, deltaTime);
//     }
//
//     public void Exit(DragonStateMachine stateMachine)
//     {
//         _currentAttackSubState?.Exit(stateMachine);
//     }
//
//     public void OnSubStateComplete()
//     {
//         // 공격 한 번만 하고 끝나면 Idle 상태로 전환
//         _stateMachine.ForceChangeState(EBossState.Idle);
//     }
//
//     private void SetAttackSubState(IEnemyState<DragonStateMachine> newSubState)
//     {
//         _currentAttackSubState?.Exit(_stateMachine);
//         _currentAttackSubState = newSubState;
//         _currentAttackSubState?.Enter(_stateMachine);
//     }
//
//     private IEnemyState<DragonStateMachine> ChooseRandomAttackState()
//     {
//         int rand = Random.Range(0, 4); // 공격 4종
//         return rand switch
//         {
//             // 0 => new DragonAttack1State(this),
//             // 1 => new DragonAttack2State(this),
//             // 2 => new DragonAttack3State(this),
//             // 3 => new DragonAttack4State(this),
//             // _ => new DragonAttack1State(this)
//         };
//     }
// }