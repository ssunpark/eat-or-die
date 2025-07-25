using Redcode.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class AItemInfo
{
    public readonly ItemData ItemData;
    private readonly Transform _poolParent;
    private Pool<Transform> _holdItemPool;
    // 아이템 프리팹 풀링

    protected AItemInfo(ItemData itemData, string prefabPath = "", Transform poolParent = null)
    {
        ItemData = itemData;
        _poolParent = poolParent;
        
        // 풀링
        var testPrefabPath = string.IsNullOrEmpty(prefabPath) ? "Weapon_Staff_Prefab" : prefabPath;
        GameObject plantPrefab = Addressables.LoadAssetAsync<GameObject>(testPrefabPath).WaitForCompletion();
        _holdItemPool = Pool.Create(plantPrefab.transform, 10, _poolParent.transform);
    }

    public GameObject GetHoldItemObject() => _holdItemPool.Get().gameObject;

    public void ReturnHoldItemToPool(GameObject item)
    {
        _holdItemPool.Take(item.transform);
        item.transform.SetParent(_poolParent);
    }
}