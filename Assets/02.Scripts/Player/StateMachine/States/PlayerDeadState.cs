using UnityEngine;
public class PlayerDeadState : APlayerStateBase
{
    public PlayerDeadState(PlayerFSM fsm) : base(fsm) {
        AnimState = "Die";
    }

    protected override void OnEnterState()
    {
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
    }

    protected override void OnEnterStateRender()
    {
        _fsm.GetComponent<ItemMagnet>().enabled = false;
        if (_fsm.HasInputAuthority)
        {
            DropAllItems();
        }
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    protected override void OnExitStateRender()
    {
    }

    protected override void OnFixedUpdate()
    {
        KCC.Move(Vector3.zero);
        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths["Die"])
        {

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
                    var item = slot.Item;
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
                    var item = slot.Item;
                    ItemManager.Instance.RPC_CreateItemObject(item.ID, item.Quantity, item.Durability, _fsm.transform.position, Quaternion.identity);
                    slot.RemoveItem();
                }
            }
        }
    }

}