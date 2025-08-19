using DarkTonic.MasterAudio;
using UnityEngine;
using Fusion.Addons.FSM;
public class PlayerDeadState : APlayerStateBase
{
    private float _selectTime = 60f;
    public PlayerDeadState(PlayerFSM fsm) : base(fsm) {
        AnimState = "Die";
        StateId = (int)EPlayerState.Dead;
    }

    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        _fsm.IsDead = true;
        
        MasterAudio.FireCustomEvent("Dead", _fsm.transform);
    }

    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
        if (_fsm.HasInputAuthority)
        {
            _fsm.ShowSelectPanel();
            _fsm.GetComponent<ItemMagnet>().enabled = false;
            DropAllItems();
        }
    }

    protected override void OnExitStateRender()
    {
    }

    protected override void OnFixedUpdateState()
    {
        if (Machine.StateTime >= _selectTime)
        {
            Machine.ForceActivateState<PlayerCorpseState>();
            return;
        }
    }


    private void DropAllItems()
    {
        _fsm.ItemHolder.SetHoldItem(null);
     
        UnifiedInventoryManager.Instance.DropAllItems(_fsm.transform.position);
    }

}