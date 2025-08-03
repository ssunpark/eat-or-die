using UnityEngine;
public class PlayerDeadState : APlayerStateBase
{
    public PlayerDeadState(PlayerFSM fsm) : base(fsm) {
    }

    private Renderer[] _rendererObjects;
    protected override void OnEnterState()
    {
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        
    }

    protected override void OnEnterStateRender()
    {
        _rendererObjects = _fsm.GetComponentsInChildren<Renderer>(true);
        if (_fsm.HasInputAuthority)
        {
            DropAllItems();
        }
        foreach (var renderer in _rendererObjects)
            renderer.gameObject.SetActive(false);
    }

    protected override void OnExitStateRender()
    {
        foreach (var renderer in _rendererObjects)
            renderer.gameObject.SetActive(true);
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
        Debug.LogWarning("PlayerDeadState: Dropping all items. This feature is not fully implemented yet.");
        // 아직 테스트 안함 -> 주석처리
        //if (InventoryManager.Instance != null)
        //{
        //    var inv = InventoryManager.Instance.Inventory;
        //    for (int i = 0; i < inv.SlotList.Count; i++)
        //    {
        //        var slot = inv.SlotList[i];
        //        if (!slot.IsEmpty)
        //        {
        //            var item = slot.Item;
        //            ItemManager.Instance.RPC_CreateItemObject(item.ID, item.Quantity, item.Durability, _controller.transform.position, Quaternion.identity);
        //            slot.RemoveItem();
        //        }
        //    }
        //}

        //if (QuickSlotManager.Instance != null)
        //{
        //    var qs = QuickSlotManager.Instance.Quickslots;
        //    for (int i = 0; i < qs.SlotList.Count; i++)
        //    {
        //        var slot = qs.SlotList[i];
        //        if (!slot.IsEmpty)
        //        {
        //            var item = slot.Item;
        //            ItemManager.Instance.RPC_CreateItemObject(item.ID, item.Quantity, item.Durability, _controller.transform.position, Quaternion.identity);
        //            slot.RemoveItem();
        //        }
        //    }
        //}
    }

}