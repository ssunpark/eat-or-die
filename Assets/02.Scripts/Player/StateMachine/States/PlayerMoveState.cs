using UnityEngine;
using Fusion.Addons.FSM;
public class PlayerMoveState : APlayerStateBase
{
    private float _hungerConsumptionOvertime;
    public PlayerMoveState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.Move;
    }

    protected override void OnInitialize()
    {
        // 전이: 방향 입력이 없으면 Idle로
        this.AddTransition(
            _controller.FSMStateInstances.Idle,
            () => _controller.GetInput(out NetworkInputData input) && input.direction.sqrMagnitude <= 0.01f
        );

        // 전이: 공격 키
        this.AddTransition(
            _controller.FSMStateInstances.Attack,
            () => CanStartAttack()
        );

        // 전이: 인터랙션 키 누르면 Interact로
        this.AddTransition(
            _controller.FSMStateInstances.Interact,
            () => _controller.GetInput(out NetworkInputData input) && input.isInteracting && _controller.CanInteract(out _)
        );

        // 전이: 아이템 사용
        this.AddTransition(
            _controller.FSMStateInstances.UseItem,
            () => _controller.GetInput(out NetworkInputData input) && input.isUsing && _controller.CanUseHeldItem(out _)
        );
    }
    protected override void OnEnterState()
    {
        _hungerConsumptionOvertime = _stat.GetStat(EStatType.HungerConsumptionOverTime);
    }

    protected override void OnFixedUpdate()
    {
        if (!_controller.GetInput(out NetworkInputData inputData))
        {
            return;
        }
        Vector3 dir = inputData.direction;
        if (dir.magnitude <= 0.01)
        {
            return;
        }
        _controller.Movement?.Move(dir, inputData.isRunning);

        _resource.ConsumeHunger(_hungerConsumptionOvertime * Machine.Runner.DeltaTime);
    }

    protected override void OnExitState()
    {
        _controller.Movement.Move(Vector3.zero, false);
    }

    private bool CanStartAttack()
    {
        if (!_controller.GetInput(out NetworkInputData input))
        {
            return false;
        }

        if (!input.isAttacking) return false;

        float cooldown = Mathf.Max(1f / _stat.GetStat(EStatType.AttackSpeed), 0.01f);
        return _controller.LastAttackTime + cooldown < Machine.Runner.LocalRenderTime;
    }
}