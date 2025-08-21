using System;
using UnityEngine;

public class PlayerRunState : APlayerStateBase
{
    private float _hungerConsumptionOvertime;
    private float _hungerConsumeReduction;
    private float _moveSpeed;
    private float _sprintMultipler;
    private float _moveExpTimer;

    public PlayerRunState(PlayerFSM fsm) : base(fsm)
    {
        AnimState = "Run";
        StateId = (int)EPlayerState.Run;
    }

    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);

        _hungerConsumptionOvertime = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.HungerConsumptionOverTime);
        _moveSpeed = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.MoveSpeed);
        _sprintMultipler = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.SprintingMultiplier);
        _hungerConsumeReduction = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.HungerConsumeReduction);

        _fsm.CanInteract = true;
        _fsm.CanUseItem = true;
    }

    protected override void OnFixedUpdateInput()
    {
        _skill?.Publish(ESkillEventType.OnMove);

        // 이동 처리
        Move();

        // Run 버튼 해제 시 → MoveState로 전환
        if (!_fsm.CurrentInput.buttons.IsSet(EButtons.Run))
        {
            RequestActivateState(EPlayerState.Move);
            return;
        }

        // 공격, 상호작용, 아이템 사용 체크
        if (_fsm.CurrentInput.buttons.WasPressed(_fsm.PreviousInput.buttons, EButtons.Attack))
        {
            RequestActivateState(EPlayerState.Attack);
            return;
        }
        if (_fsm.CurrentInput.buttons.WasPressed(_fsm.PreviousInput.buttons, EButtons.Interact) && IsInteractTargetExists())
        {
            RequestActivateState(EPlayerState.Interact);
            return;
        }
        if (_fsm.CurrentInput.buttons.WasPressed(_fsm.PreviousInput.buttons, EButtons.UseItem) && IsUseItemTargetExists())
        {
            RequestActivateState(EPlayerState.UseItem);
            return;
        }
    }

    private void Move()
    {
        var moveInput = _fsm.CurrentInput.direction;
        if (moveInput.sqrMagnitude < 0.01f)
        {
            RequestActivateState(EPlayerState.Idle);
            KCC.Move(Vector3.zero);
            return;
        }

        Vector2 normalized = moveInput.normalized;
        Vector3 movementDirection = new Vector3(normalized.x, 0, normalized.y);

        if (movementDirection.sqrMagnitude > 0.001f)
        {
            KCC.SetLookRotation(Quaternion.LookRotation(movementDirection));
        }

        KCC.Move(movementDirection * _moveSpeed * _sprintMultipler, 0);
    }

    protected override void OnFixedUpdateState()
    {
        if (!_fsm.PlayerNetworkObject.HasInputAuthority)
            Move();

        _resource.ConsumeHunger((_hungerConsumptionOvertime - _hungerConsumeReduction) * 2 * Machine.Runner.DeltaTime);

        _moveExpTimer += Machine.Runner.DeltaTime;
        if (_moveExpTimer >= 1f)
        {
            GrantExpOrder("RunPerSecond");
            _moveExpTimer = 0f;
        }
    }
}
