using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ItemMagnet : NetworkBehaviour
{
    [SerializeField]
    private float _absorbRadius = 3f; // 흡수 범위
    [SerializeField]
    private LayerMask _itemLayerMask; // 아이템 레이어
    
    private void Update()
    {
        Pick();
    }

    private void Pick()
    {
        var items = DetectPickableItems();
        foreach (var item in items)
        {
            // 로컬 조건 체크 (인벤토리 매니저)
            // 서버 조건 체크 (인벤토리 매니저 RPC) => 아이템 스택의 오너가 설정됨
            var networkItem = item.GetComponent<NetworkObject>();
            RPC_RequestPick(networkItem.Id, Room.Instance.Runner.LocalPlayer);
            // 아이템 흡수 연출 시작
            var pickableItem = item.GetComponent<IPickable>();
            pickableItem.Pick();
            // 인벤토리 등록
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestPick(NetworkId itemId, PlayerRef player)
    {
        var itemObject = Room.Instance.Runner.FindObject(itemId)?.GetComponent<ItemObject>();

        if (itemObject == null)
        {
            Debug.LogWarning("아이템이 존재하지 않음");
            return;
        }

        if (itemObject.HasOwner)
        {
            Debug.Log("이미 다른 플레이어가 주움");
            return;
        }

        itemObject.HasOwner = true;

        // 인벤토리 추가 등 서버 로직
        Debug.Log($"서버: {player} 가 아이템 {itemObject.ItemID} {itemObject.Quantity}개 주움");
    }
    
    private List<GameObject> DetectPickableItems()
    {
        var pickableList = new List<GameObject>();
        var colliders = Physics.OverlapSphere(transform.position, _absorbRadius, _itemLayerMask);

        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out IPickable pickable))
            {
                pickableList.Add(col.gameObject);
            }
        }

        return pickableList;
    }
}