using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdleState : APlayerStateBase
{
    public PlayerIdleState(PlayerFSM controller) : base(controller)
    {
        AnimState = "Idle";
        StateId = (int)EPlayerState.Idle;
    }

    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();

        _fsm.CanInteract = true;
        _fsm.CanUseItem = true;
    }

    protected override void OnEnterState()
    {
        base.OnEnterState();
    }

    protected override void PreFixedUpdate()
    {
        KCC.Move(Vector3.zero);
    }
    protected override void OnFixedUpdateInput()
    {
        _skill?.Publish(ESkillEventType.OnIdle);

        if (_fsm.CurrentInput.IsUnityNull())
        {
            return;
        }
        var input = _fsm.CurrentInput.direction;
        
        if (!Mathf.Approximately(input.sqrMagnitude, 0f))
        {
            if(_fsm.CurrentInput.buttons.IsSet(EButtons.Run))
            {
                RequestActivateState(EPlayerState.Run);
                return;
            }
            RequestActivateState(EPlayerState.Move);
            return;
        }
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


}