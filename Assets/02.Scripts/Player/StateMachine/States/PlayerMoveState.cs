using UnityEngine;

public class PlayerMoveState : APlayerState
{
    public PlayerMoveState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {
    }
    public override void Enter()
    {
        _moveSatietyTimer = _fsm.MoveSatietyTimer;
        _moveStatietyInterval = _fsm.MoveStatietyInterval;
    }
    public override bool CanMove => true;
    public override bool CanAct => true;
    private float _moveSatietyTimer;
    private float _moveStatietyInterval;

    public override void Tick()
    {
        if (!_controller.GetInput(out NetworkInputData inputData))
        {
            _fsm.ChangeState(EPlayerState.Idle);
            return;
        }

        if (inputData.isAttacking)
        {
            if (CanAttack)
            {
                _fsm.ChangeState(EPlayerState.Attack);
                return;
            }
        }
        if (inputData.isInteracting)
        {
            IInteractable interactable;
            if (_fsm.Interact.TryInteract(out interactable))
            {
                _fsm.Interactable = interactable;
                _fsm.ChangeState(EPlayerState.Interact);
                return;
            }
        }
        if (inputData.isUsing)
        {
            IUsable usable;
            if(_fsm.Interact.TryUseItem(out usable))
            {
                _fsm.Usable = usable;
                _fsm.ChangeState(EPlayerState.UsingTool);
                return;
            }
        }

        Vector3 dir = inputData.direction;

        _moveSatietyTimer += _fsm.Runner.DeltaTime;
        if (_moveSatietyTimer >= _moveStatietyInterval)
        {
            float rate = _stat.GetStat(EStatType.ConsumptionRate);
            _resource.ConsumeSatiety(_fsm.Runner.DeltaTime * _stat.GetStat(EStatType.ConsumptionRate));
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
    }
}