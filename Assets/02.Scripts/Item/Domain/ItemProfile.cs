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
    
    // 아이템 기능 실행 파이프라인
    // 먹기, 설치, 상호작용
    private readonly IItemUsePipeline _pipeline;

    public ItemProfile(ItemDefinition itemDefinition, List<IItemHoldEffect> holdEffect, IItemUsePipeline pipeline, Pool<Transform> prefabPool, Transform poolParent)
    {
        ItemDefinition = itemDefinition;
        _holdEffect = holdEffect;
        _itemPrefabPool = prefabPool;
        _poolParent = poolParent;
        _pipeline = pipeline;
    }

    public void HoldItem(GameObject target)
    {
        foreach (var effect in _holdEffect)
        {
            effect.Hold(target);
        }
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
        _pipeline?.Run(target);
        
        return true;
    }
}