using DarkTonic.MasterAudio;
using Fusion.Addons.FSM;
using UnityEngine;
using System.Linq;
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
        
        MasterAudio.PlaySound3DAtTransform("Dead", _fsm.transform);
    }

    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
        if (_fsm.HasInputAuthority)
        {
            var players = PlayerInfoManager.PlayerControllers.Values;
            bool allDead = players.Where(p => p != null && p.PlayerFSM != null).All(p => p.PlayerFSM.IsDead);

            if (!allDead)
            {
                _fsm.ShowSelectPanel();
            }
            DropAllItems();
        }
    }

    protected override void OnExitStateRender()
    {
    }

    protected override void OnFixedUpdateState()
    {
        if (Machine.StateTime >= _selectTime && !_fsm.IsInReviveProcess)
        {
            Machine.ForceActivateState<PlayerCorpseState>();
            return;
        }
    }


    private void DropAllItems()
    {
        _fsm.ItemHolder.SetHoldItem(null);

        _fsm.GetComponent<ItemMagnet>().enabled = false;
        UnifiedInventoryManager.Instance.DropAllItems(_fsm.transform.position);
    }

}