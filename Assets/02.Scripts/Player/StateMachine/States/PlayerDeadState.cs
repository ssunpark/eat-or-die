using UnityEngine;
using Fusion.Addons.FSM;
public class PlayerDeadState : APlayerStateBase, IAnimationActionEndNotify
{
    public PlayerDeadState(PlayerController controller) : base(controller) {
        StateId = (int)EPlayerState.Dead;
    }

    protected override void OnEnterState()
    {
        _controller.Movement.Move(Vector3.zero, false);

        if(_controller.Object.HasInputAuthority)
        {
            InputReader.playerControllerInputBlocked = true;
        }

        _controller.PlayAnimTrigger(EAnimTrigger.Die);
        _controller.SetMoveFlagNetwork(true);
    }
    protected override bool CanExitState(IState nextState)
    {
        return false;
    }



    private void DropAllItems()
    {
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

    void IAnimationActionEndNotify.OnAnimationFinished()
    {
        //SpectatorManager.Instance?.StartSpectate();

        DropAllItems();
        // SpawnCorpse();
    }
}