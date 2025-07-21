using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    public PlayerMoveState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {
    }
    public override void Enter()
    {
        _moveSatietyTimer = _fsm.MoveSatietyTimer;
        _moveStatietyInterval = _fsm.MoveStatietyInterval;
    }

    private float _moveSatietyTimer;
    private float _moveStatietyInterval;

    public override void Tick()
    {
        if (!_controller.GetInput(out NetworkInputData inputData)) return;

        Vector3 dir = inputData.direction;

        _moveSatietyTimer += Time.deltaTime;
        if (_moveSatietyTimer >= _moveStatietyInterval)
        {
            float rate = _stat.GetStat(EStatType.ConsumptionRate);
            _resource.ConsumeSatiety(Time.deltaTime * _stat.GetStat(EStatType.ConsumptionRate));
            _moveSatietyTimer = 0f;
        }

        if (dir.sqrMagnitude <= 0.01f)
        {
            _fsm.ChangeState(EPlayerState.Idle);
        }
    }

    public override void Exit()
    {
        _fsm.MoveSatietyTimer = _moveSatietyTimer;
        _controller.Stop();
    }
}