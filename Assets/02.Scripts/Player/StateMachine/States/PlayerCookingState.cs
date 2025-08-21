using Fusion;
using UnityEngine;
public class PlayerCookingState : APlayerStateBase
{
    private bool _isCookCompleted = false;
    private float _cookTime = 4f;
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
        _isCookCompleted = false;
        _fsm.Spoon.SetActive(true);
    }

    protected override void OnFixedUpdateInput()
    {
        KCC.Move(Vector3.zero);

        if (Machine.StateTime >= _cookTime && !_isCookCompleted)
        {
            _isCookCompleted = true;
            CookingManager.Instance.OnCookingCompleted(true);
            Anim.CrossFadeInFixedTime("Cook_Success", AnimTransitionLength);

            GrantExpOrder("RetrieveCookedFood");
            RequestActivateState();
        }
    }
    protected override void OnExitStateRender()
    {
        if (_fsm.HasInputAuthority)
        {
            if (_isCookCompleted)
            {
                _isCookCompleted = false;
                return;
            }
            CookingManager.Instance.OnCookingCompleted(false);
            _isCookCompleted = false;
        }

        _fsm.Spoon.SetActive(false);
    }
}
