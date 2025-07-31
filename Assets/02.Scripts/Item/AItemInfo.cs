using System.Collections.Generic;
using Redcode.Pools;
using UnityEngine;

// 아이템의 정적 정보와 동작을 정의
public class AItemInfo
{
    public readonly ItemData ItemData;
    private readonly Transform _poolParent;
    private Pool<Transform> _holdItemPool;
    
    // 장착 시 효과
    // 스텟 변경, 상호작용 태그 변경
    private readonly List<IItemHoldEffect> _holdEffect;
    
    // 아이템 기능
    // 먹기, 설치, 상호작용
    private readonly List<IUseEffect> _useEffect;

    public AItemInfo(ItemData itemData, List<IItemHoldEffect> holdEffect, List<IUseEffect> useEffect, Transform poolParent, List<string> extraDescription = null)
    {
        ItemData = itemData;
        _useEffect = useEffect;
        _holdEffect = holdEffect;
        _poolParent = poolParent;

        if (extraDescription != null)
        {
            foreach (var description in extraDescription)
            {
                ItemData.AddDescription(description);
            }
        }

        // 풀링
        _holdItemPool = Pool.Create(ItemData.Prefab.transform, 10, _poolParent.transform);
    }

    public void HoldItem(GameObject target)
    {
        foreach (var effect in _holdEffect)
        {
            effect.Hold(target);
        }
    }

    public GameObject GetHoldItemObject() => _holdItemPool.Get().gameObject;

    public void ReturnHoldItemToPool(GameObject item)
    {
        _holdItemPool.Take(item.transform);
        item.transform.SetParent(_poolParent);
    }

    public void UseItem(GameObject target)
    {
        foreach (var effect in _useEffect)
        {
            effect.Use(target);
        }
    }
}