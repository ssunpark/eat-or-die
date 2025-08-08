using System.Collections.Generic;
using Redcode.Pools;
using UnityEngine;

// 아이템의 정적 정보와 동작을 정의
public class ItemProfile
{
    public readonly ItemDefinition ItemDefinition;
    private readonly Transform _poolParent;
    private Pool<Transform> _itemPrefabPool;
    
    // 장착 시 효과
    // 스텟 변경, 상호작용 태그 변경
    private readonly List<IItemHoldEffect> _holdEffect;
    
    // 아이템 기능
    // 먹기, 설치, 상호작용
    private readonly List<IUseEffect> _useEffect;

    public ItemProfile(ItemDefinition itemDefinition, List<IItemHoldEffect> holdEffect, List<IUseEffect> useEffect, Pool<Transform> prefabPool, Transform poolParent, List<string> extraDescription = null)
    {
        ItemDefinition = itemDefinition;
        _useEffect = useEffect;
        _holdEffect = holdEffect;
        _itemPrefabPool = prefabPool;
        _poolParent = poolParent;

        if (extraDescription != null)
        {
            foreach (var description in extraDescription)
            {
                ItemDefinition.AddDescription(description);
            }
        }

        // 풀링
        _itemPrefabPool = Pool.Create(ItemDefinition.Prefab.transform, 0, _poolParent.transform);
    }

    public void HoldItem(GameObject target)
    {
        foreach (var effect in _holdEffect)
        {
            effect.Hold(target);
        }
        // 아이템 오브젝트 장착
    }

    public void UnHoldItem(GameObject target, GameObject item)
    {
        foreach (var effect in _holdEffect)
        {
            effect.UnHold(target);
        }

        ReturnHoldItemToPool(item);
    }

    public GameObject GetHoldItemObject() => _itemPrefabPool.Get().gameObject;

    public void ReturnHoldItemToPool(GameObject item)
    {
        if (item == null)
        {
            return;
        }
        
        _itemPrefabPool.Take(item.transform);
        item.transform.SetParent(_poolParent);
    }

    public bool TryUseItem(GameObject target)
    {
        if (_useEffect.Count <= 0)
        {
            return false;
        }

        foreach (var effect in _useEffect)
        {
            effect.Use(target);
        }
        
        return true;
    }
}