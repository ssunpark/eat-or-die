using Fusion;
using UnityEngine;
public class PlayerCookingState : APlayerStateBase
{
    private bool _isCookCompleted = false;
    public PlayerCookingState(PlayerFSM controller) : base(controller) 
    {
        AnimState = "Cook";
        StateId = (int)EPlayerState.Cooking;
    }
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;

    }
    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
    }

    protected override void OnFixedUpdateInput()
    {
        KCC.Move(Vector3.zero);

        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState]*2 && !_isCookCompleted)
        {
            _isCookCompleted = true;
            CookingManager.Instance.OnCookingCompleted();

            GrantExpOrder("RetrieveCookedFood");
            RequestActivateState();
        }
    }
    protected override void OnExitStateRender()
    {
        if (_fsm.HasInputAuthority)
        {
            if (_isCookCompleted) return;
            CookingManager.Instance.OnCookingCompleted();
            _isCookCompleted = false;
        }
    }
}
