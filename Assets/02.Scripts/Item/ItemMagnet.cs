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
        if (!GetComponent<NetworkObject>().HasInputAuthority)
        {
            return;
        }
        
        var items = DetectPickableItems();
        foreach (var item in items)
        {
            // 로컬 조건 체크 (인벤토리 매니저)
            if (item.GetComponent<ItemObject>().HasOwner)
            {
                continue;
            }
            // 서버 조건 체크 (인벤토리 매니저 RPC) => 아이템 스택의 오너가 설정됨
            var networkItem = item.GetComponent<NetworkObject>();

            var vectorToItem = networkItem.transform.position;
            RPC_RequestPick(networkItem.Id, Runner.LocalPlayer, transform.position);
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestPick(NetworkId itemId, PlayerRef player, Vector3 playerPos)
    {
        var itemObject = Runner.FindObject(itemId)?.GetComponent<ItemObject>();

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
        var itemPos = itemObject.transform.position;
        Debug.Log($"요청 플레이어 위치: {playerPos}, 아이템 위치: {itemPos}");
        if(Vector3.Distance(itemPos, playerPos) > _absorbRadius)
        {
            Debug.Log("아이템이 너무 멀리 있음");
            return;
        }

        itemObject.HasOwner = true;
        
        // 아이템 흡수 연출 시작
        itemObject.RPC_Pick(GetComponent<NetworkObject>().Id);

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