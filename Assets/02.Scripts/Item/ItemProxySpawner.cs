using Fusion;
using UnityEngine;

public class ItemProxySpawner : NetworkBehaviourSingleton<ItemProxySpawner>
{
    [Header("아이템 오브젝트")]
    [SerializeField]
    private NetworkPrefabRef _itemObjectPrefab;
    
    /// <summary>
    /// 아이템 생성(드랍)
    /// </summary>
    /// <param name="id">아이템 ID</param>
    /// <param name="quantity">수량</param>
    /// <param name="position">생성 위치</param>
    /// <param name="rotation">생성 시 각도</param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_CreateItemObject(int id, int quantity, float durability, Vector3 position, Quaternion rotation, string extraInfo = "", float pickableTime = 4f)
    {
        if (!Runner.IsServer)
        {
            return;
        }
        
        if (!ItemManager.Instance.ItemDictionary.TryGetValue(id, out ItemProfile item))
        {
            Debug.LogWarning($"없는 아이템입니다. ID: {id}");
            return;
        }
        
        // 네트워크 아이템 오브젝트 생성
        Runner.Spawn(_itemObjectPrefab,
            position: position,
            rotation: rotation,
            inputAuthority: null,
            onBeforeSpawned: (runner, obj) =>
            {
                var item = obj.GetComponent<ItemObject>();
                item.ItemID = id;
                item.Quantity = quantity;
                item.SpawnPosition = position;
                item.Durability = durability;
                item.ExtraInfo = extraInfo;
                item.PickableTime = pickableTime;
            });
    }
}