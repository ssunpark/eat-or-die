using UnityEngine;
using Fusion.Addons.FSM;
public class PlayerDeadState : APlayerStateBase
{
    public PlayerDeadState(PlayerFSM fsm) : base(fsm) {
        AnimState = "Die";
        StateId = (int)EPlayerState.Dead;
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
        if (_fsm.HasInputAuthority)
        {
            _fsm.GetComponent<ItemMagnet>().enabled = false;
            DropAllItems();
        }
    }

    protected override void OnExitStateRender()
    {
    }

    protected override void OnFixedUpdateState()
    {
        KCC.Move(Vector3.zero);
        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths["Die"])
        {
            Machine.ForceActivateState<PlayerCorpseState>();
            return;
        }
    }


    private void DropAllItems()
    {
        _fsm.ItemHolder.SetHoldItem(null);
        if (InventoryManager.Instance != null)
        {
            var inv = InventoryManager.Instance.Inventory;
            for (int i = 0; i < inv.SlotList.Count; i++)
            {
                var slot = inv.SlotList[i];
                if (!slot.IsEmpty)
                {
                    var item = slot.ItemInstance;
                    ItemManager.Instance.RPC_CreateItemObject(item.ID, item.Quantity, item.Durability, _fsm.transform.position, Quaternion.identity);
                    slot.RemoveItem();
                }
            }
        }

        if (QuickSlotManager.Instance != null)
        {
            var qs = QuickSlotManager.Instance.Quickslots;
            for (int i = 0; i < qs.SlotList.Count; i++)
            {
                var slot = qs.SlotList[i];
                if (!slot.IsEmpty)
                {
                    var item = slot.ItemInstance;
                    ItemManager.Instance.RPC_CreateItemObject(item.ID, item.Quantity, item.Durability, _fsm.transform.position, Quaternion.identity);
                    slot.RemoveItem();
                }
            }
        }
    }

}