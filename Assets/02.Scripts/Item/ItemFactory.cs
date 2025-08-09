using System.Collections.Generic;
using Redcode.Pools;
using Unity.VisualScripting;
using UnityEngine;

public class ItemFactory
{
    private readonly Transform _itemPoolParent;
    
    private readonly Dictionary<string, Pool<Transform>> _sharedPools = new();

    public ItemFactory(Transform itemPoolParent)
    {
        _itemPoolParent = itemPoolParent;
    }

    // 음식 아이템 효과 설명 factory
    private EatEffectManager _eatEffectManager = new();

    // 주어진 데이터에 맞게 아이템 생성 후 반환
    public ItemProfile CreateItem(EatItemRawData rawData)
    {
        var holdEffectList = new List<IItemHoldEffect>();
        var effectList = new List<IUseEffect>();
        var extraDescription = new List<string>();

        // 기본 배고픔
        var hungerEffect = new EatEffect_HungerInstantRecovery(rawData.HungerRestore);
        effectList.Add(hungerEffect);
        extraDescription.Add(hungerEffect.Description);

        var rawEffects = new (EStatType? type, float? value, float? duration)[]
        {
            (rawData.EffectType1, rawData.Value1, rawData.Duration1),
            (rawData.EffectType2, rawData.Value2, rawData.Duration2),
            (rawData.EffectType3, rawData.Value3, rawData.Duration3),
        };

        // 섭취 버프
        foreach (var (type, value, duration) in rawEffects)
        {
            if (type is EStatType statType)
            {
                var statValue = value ?? 0;
                var buffDuration = duration ?? 0;
                var modifierType = _eatEffectManager.GetStatModifierType(statType);
                var effect = new EatEffect_StatModifier(statType, statValue, buffDuration, modifierType);
                effectList.Add(effect);
                var desc = _eatEffectManager.GetDescription(statType, statValue, buffDuration);
                extraDescription.Add(desc);
            }
        }
        
        // HOld 효과 정의
        holdEffectList.Add(new ItemHoldEffect_InteractionTag(rawData.InteractionTag));
        holdEffectList.Add(new ItemHoldEffect_Animator("Food"));
        var itemData = new ItemDefinition(rawData.ID, rawData.Name, rawData.Description, rawData.IsIngredient, false,
            rawData.MaxQuantity, 1f, EAttackType.MeleeWeapon, rawData.IconPath, rawData.PrefabPath);
        
        var (pool, poolParent) = GetOrCreateSharedPool(rawData.PrefabPath, itemData.Prefab, _itemPoolParent);
        return new ItemProfile(itemData, holdEffectList, effectList, pool, poolParent, extraDescription);
    }

    public ItemProfile CreateItem(WeaponItemRawData rawData)
    {
        var itemData = new ItemDefinition(rawData.ID, rawData.Name, rawData.Description, rawData.IsIngredient, true,
            rawData.MaxStack, rawData.MaxDuration, rawData.AttackType,
            rawData.IconPath, rawData.PrefabPath, rawData.ProjectileKey);
        
        // HOld 효과 정의
        var holdStatEffect = new ItemHoldEffect_Weapon(rawData.MeleeDamage, rawData.MagicDamage, rawData.AttackSpeed, rawData.Range);
        var holdAnimatorEffect = new ItemHoldEffect_Animator(rawData.ActionName);
        var holdEffectList = new List<IItemHoldEffect>() { holdStatEffect, holdAnimatorEffect };
        
        var (pool, poolParent) = GetOrCreateSharedPool(rawData.PrefabPath, itemData.Prefab, _itemPoolParent);
        return new ItemProfile(itemData, holdEffectList, null, pool, poolParent);
    }

    public ItemProfile CreateItem(UsableItemRawData rawData)
    {
        var itemData = new ItemDefinition(rawData.ID, rawData.Name, rawData.Description, false, rawData.HasDurability, rawData.MaxQuantity,
            rawData.MaxDuration ?? 1f, EAttackType.MeleeWeapon,
            rawData.AddressablePath, rawData.PrefabPath);

        IUseEffect useEffect = rawData.ActionName switch
        {
            "Hoe" => new UseEffectHoe(),
            "WateringCan" => new UseEffectWateringCan(),
            "Seed" => new UseEffectSeed(rawData.ID),
            _ => new UseEffectNone()
        };
        var effectList = new List<IUseEffect>() { useEffect };
        
        // HOld 효과 정의
        var holdAnimatorEffect = new ItemHoldEffect_Animator(rawData.ActionName);
        var holdInteractionEffect = new ItemHoldEffect_InteractionTag(rawData.InteractionTag);
        var holdEffectList = new List<IItemHoldEffect>() { holdAnimatorEffect, holdInteractionEffect };
        
        var (pool, poolParent) = GetOrCreateSharedPool(rawData.PrefabPath, itemData.Prefab, _itemPoolParent);
        return new ItemProfile(itemData, holdEffectList, effectList, pool, poolParent);
    }
    
    private (Pool<Transform>, Transform) GetOrCreateSharedPool(string key, GameObject prefab, Transform poolParent)
    {
        if (_sharedPools.TryGetValue(key, out var existingPool))
            return (existingPool, existingPool.Container);

        GameObject parent = new GameObject(key);
        parent.transform.SetParent(poolParent);
        var newPool = Pool.Create(prefab.transform, 0, parent.transform);
        _sharedPools[key] = newPool;
        return (newPool, parent.transform);
    }
}