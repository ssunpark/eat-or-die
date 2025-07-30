using UnityEngine;

public class PlayerDeadState : APlayerStateBase, IAnimationActionEndNotify
{
    public PlayerDeadState(PlayerController controller) : base(controller) {
        StateId = (int)EPlayerState.Dead;
    }

    protected override void OnEnterState()
    {
        _controller.Movement.Move(Vector3.zero, false);
        DropAllItems();
        InputReader.playerControllerInputBlocked = true;

        if(_controller.Object.HasInputAuthority)
        {
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Die);
            _controller.RPC_SetMoveFlag(true);
        }
    }

    private void DropAllItems()
    {
        if (InventoryManager.Instance != null)
        {
            var inv = InventoryManager.Instance.Inventory;
            for (int i = 0; i < inv.SlotList.Count; i++)
            {
                var slot = inv.SlotList[i];
                if (!slot.IsEmpty)
                {
                    var item = slot.Item;
                    ItemManager.Instance.RPC_CreateItemObject(item.ID, item.Quantity, item.Durability, _controller.transform.position, Quaternion.identity);
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
                    ItemManager.Instance.RPC_CreateItemObject(item.ID, item.Quantity, item.Durability, _controller.transform.position, Quaternion.identity);
                    slot.RemoveItem();
                }
            }
        }
    }

    void IAnimationActionEndNotify.OnAnimationFinished()
    {
        SpectatorManager.Instance?.StartSpectate();

        // SpawnCorpse();
    }
}