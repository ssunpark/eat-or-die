using UnityEngine;
using Fusion.Addons.FSM;
using Fusion;
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
            _controller.FSMStateInstances.Idle, EvaluateMove);

        // 전이: 공격 키
        this.AddTransition(
            _controller.FSMStateInstances.Attack, CanStartAttack);

        // 전이: 인터랙션 키 누르면 Interact로
        this.AddTransition(_controller.FSMStateInstances.Interact, CanInteract);

        // 전이: 아이템 사용
        this.AddTransition(_controller.FSMStateInstances.UseItem, CanUseItem);
    }

    private bool EvaluateMove()
    {
        if (!_controller.HasStateAuthority) return false;
        if (!TryCacheInput()) return false;
        return _input.direction.sqrMagnitude <= 0.01f;
    }

    protected override void OnEnterState()
    {
        if (_controller.HasStateAuthority)
        {
            _controller.IsMoving = true;
        }
        _hungerConsumptionOvertime = _stat.GetStat(EStatType.HungerConsumptionOverTime);
    }

    protected override void OnFixedUpdate()
    {
        if (!TryCacheInput())
        {
            return;
        }
        Vector3 dir = _input.direction;
        if (dir.magnitude <= 0.01)
        {
            return;
        }
        _controller.Movement?.Move(dir, _input.buttons.IsSet(EButtons.Run));
        if(_controller.HasStateAuthority)
        {
            _controller.PlayerAnimatorController.RPC_SetMoveSpeed(_controller.GetComponent<NetworkCharacterController>().Velocity.magnitude);
        }
        _resource.ConsumeHunger(_hungerConsumptionOvertime * Machine.Runner.DeltaTime);
    }

    protected override void OnExitState()
    {
        if (_controller.HasStateAuthority)
        {
            _controller.PlayerAnimatorController.MoveSpeed = 0;
            _controller.IsMoving = false;
            _controller.Movement.Move(Vector3.zero, false);
        }
    }
}