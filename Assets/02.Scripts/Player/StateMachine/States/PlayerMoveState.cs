using UnityEngine;
using Fusion.Addons.FSM;
public class PlayerMoveState : APlayerStateBase
{
    private float _moveHungerTimer;
    private float _moveStatietyInterval;
    public PlayerMoveState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.Move;
    }
    protected override void OnEnterState()
    {
        _moveHungerTimer = _controller.StateValues.MoveHungerTimer;
        _moveStatietyInterval = _controller.StateValues.MoveHungerInterval;
    }

    protected override void OnFixedUpdate()
    {
        if (!_controller.GetInput(out NetworkInputData inputData))
        {
            Machine.ForceActivateState((int)EPlayerState.Idle);
            return;
        }

        Vector3 dir = inputData.direction;

        _controller.Movement?.Move(dir, inputData.isRunning);

        _moveHungerTimer += Machine.Runner.DeltaTime;
        if (_moveHungerTimer >= _controller.StateValues.MoveHungerInterval)
        {
            _resource.ConsumeHunger(Machine.Runner.DeltaTime * _stat.GetStat(EStatType.HungerConsumptionOverTime));
            _moveHungerTimer = 0f;
        }
        if (PlayerFSMTransitionEvaluator.Evaluate(_controller, inputData, Machine.Runner, out var next))
        {
            Machine.ForceActivateState(next);
            return;
        }
    }

    protected override void OnExitState()
    {
        _controller.StateValues.MoveHungerTimer = _moveHungerTimer;
        _controller.Movement.Move(Vector3.zero, false);
    }
}