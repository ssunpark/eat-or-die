using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
public class PlayerMoveState : APlayerStateBase
{
    private float _hungerConsumptionOvertime; 
    private float _moveSpeed;
    private float _sprintMultipler;
    private float _moveExpTimer;
    public PlayerMoveState(PlayerFSM fsm) : base(fsm)
    {
        AnimState = "Move";
        StateId = (int)EPlayerState.Move;
    }


    protected override void OnEnterState()
    {
        base.OnEnterState();
        
    }

    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
        _hungerConsumptionOvertime = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.HungerConsumptionOverTime);
        _moveSpeed = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.MoveSpeed);
        _sprintMultipler = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.SprintingMultiplier);
        _fsm.CanInteract = true;
        _fsm.CanUseItem = true;
        
    }

    protected override void OnFixedUpdateInput()
    {
        _skill?.Publish(ESkillEventType.OnMove, _skill.Context);
        Move();
        if (_fsm.CurrentInput.buttons.WasPressed(_fsm.PreviousInput.buttons, EButtons.Attack))
        {
            RequestActivateState(EPlayerState.Attack);
            return;
        }
        if (_fsm.CurrentInput.buttons.WasPressed(_fsm.PreviousInput.buttons, EButtons.Interact))
        {
            if (IsInteractTargetExists())
            {
                RequestActivateState(EPlayerState.Interact);
                return;
            }
        }
        if (_fsm.CurrentInput.buttons.WasPressed(_fsm.PreviousInput.buttons, EButtons.UseItem))
        {
            if (IsUseItemTargetExists())
            {
                RequestActivateState(EPlayerState.UseItem);
                return;
            }
        }
    }

    protected void Move()
    {
        float multiplier = _fsm.CurrentInput.buttons.IsSet(EButtons.Run) ? _sprintMultipler : 1f;

        var moveInput = _fsm.CurrentInput.direction;

        if (moveInput.sqrMagnitude < 0.01f)
        {
            RequestActivateState();
            KCC.Move(Vector3.zero);
            return;
        }
        

        Vector2 normalized = moveInput.normalized;
        Vector3 movementDirection = new Vector3(normalized.x, 0, normalized.y);

        if (movementDirection.sqrMagnitude > 0.001f)
        {
            KCC.SetLookRotation(Quaternion.LookRotation(movementDirection));
        }

        KCC.Move(movementDirection * _moveSpeed * multiplier, 0);
    }
    protected override void OnFixedUpdateState()
    {
        if(!_fsm.PlayerNetworkObject.HasInputAuthority)
        {
            Move();
        }
        _resource.ConsumeHunger(_hungerConsumptionOvertime * Machine.Runner.DeltaTime);
        _moveExpTimer += Machine.Runner.DeltaTime;
        if (_moveExpTimer >= 1f)
        {
            GrantExpOrder("MovePerSecond");
            _moveExpTimer = 0f;
        }
    }

    protected override void OnExitState()
    {
    }
}