using System.Collections.Generic;
using Redcode.Pools;
using UnityEngine;

public class ItemFactory
{
    private const string FOOD_EFFECT_CSV_PATH = "/ItemCSV/FoodEffect.csv";
    
    private readonly Transform _itemPoolParent;

    private readonly Dictionary<string, Pool<Transform>> _sharedPools = new();

    private Dictionary<EStatType, EStatModifierType> _foodEffectModifierDictionary;
    
    // 아이템 효과 설명 factory
    private ItemExtraDescriptionFactory _itemExtraDescriptionFactory = new();
    
    public ItemFactory(Transform itemPoolParent)
    {
        _itemPoolParent = itemPoolParent;
        LoadFoodEffectModifier();
    }

    // 음식 아이템 효과 연산자
    private void LoadFoodEffectModifier()
    {
        _foodEffectModifierDictionary = new Dictionary<EStatType, EStatModifierType>();
        var effectList =
            CSVLoader<FoodEffectRawData>.LoadCSV($"{Application.streamingAssetsPath}{FOOD_EFFECT_CSV_PATH}");
        foreach (var effect in effectList)
        {
            _foodEffectModifierDictionary.Add(effect.StatType, effect.StatModifierType);
        }
    }

    // 주어진 데이터에 맞게 아이템 생성 후 반환
    public ItemProfile CreateItem(EatItemRawData rawData)
    {
        var holdEffectList = new List<IItemHoldEffect>();
        var extraDescription = new List<string>();

        // 기본 배고픔
        var hungerEffect = new EatEffect_HungerInstantRecovery(rawData.HungerRestore);
        extraDescription.Add(hungerEffect.Description);

        var rawEffects = new (EStatType? type, float? value, float? duration)[]
        {
            (rawData.EffectType1, rawData.Value1, rawData.Duration1),
            (rawData.EffectType2, rawData.Value2, rawData.Duration2),
            (rawData.EffectType3, rawData.Value3, rawData.Duration3),
        };

        // 섭취 버프
        var effectList = new List<IUseEffect>();
        foreach (var (type, value, duration) in rawEffects)
        {
            if (type is EStatType statType)
            {
                var statValue = value ?? 0;
                var buffDuration = duration ?? 0;
                if (_foodEffectModifierDictionary.TryGetValue(statType, out var modifier))
                {
                    var effect = new EatEffect_StatModifier(rawData.ID);
                    effectList.Add(effect);
                }
                
                var desc = _itemExtraDescriptionFactory.GetDescription(EItemType.Food, statType, "#ffffff", statValue, buffDuration);
                extraDescription.Add(desc);
            }
        }
        
        // HOld 효과 정의
        holdEffectList.Add(new ItemHoldEffect_InteractionTag(rawData.InteractionTag));
        holdEffectList.Add(new ItemHoldEffect_Animator("Food"));

        var itemDefinition = new ItemDefinition(rawData.ID, rawData.Name, rawData.Description, EItemType.Food,
            extraDescription: extraDescription,
            isIngredient: rawData.IsIngredient,
            maxQuantity: rawData.MaxQuantity,
            iconAddressablePath: rawData.IconPath,
            prefabAddressablePath: rawData.PrefabPath);

        var (pool, poolParent) = GetOrCreateSharedPool(rawData.PrefabPath, itemDefinition.Prefab, _itemPoolParent);

        var pipeline = new ItemEatEffectPipeline(hungerEffect, effectList, rawData.IsIngredient);
        return new ItemProfile(itemDefinition, holdEffectList, pipeline, pool, poolParent);
    }

    public ItemProfile CreateItem(WeaponItemRawData rawData)
    {
        var extraDescription = new List<string>()
        {
            _itemExtraDescriptionFactory.GetDescription(EItemType.Weapon, EStatType.MeleeDamage, "#ffffff", rawData.MeleeDamage),
            _itemExtraDescriptionFactory.GetDescription(EItemType.Weapon, EStatType.MagicDamage, "#ffffff", rawData.MagicDamage),
            _itemExtraDescriptionFactory.GetDescription(EItemType.Weapon, EStatType.AttackSpeed, "#ffffff", rawData.AttackSpeed),
            _itemExtraDescriptionFactory.GetDescription(EItemType.Weapon, EStatType.AttackRange, "#ffffff", rawData.AttackRange),
        };
        
        var itemDefinition = new ItemDefinition(rawData.ID, rawData.Name, rawData.Description, EItemType.Weapon,
            extraDescription: extraDescription,
            isIngredient: rawData.IsIngredient,
            maxQuantity: rawData.MaxQuantity,
            maxDurability: rawData.MaxDuration,
            attackType: rawData.AttackType,
            iconAddressablePath: rawData.IconPath,
            prefabAddressablePath: rawData.PrefabPath,
            projectileKey: rawData.ProjectileKey);

        // HOld 효과 정의
        var holdEffectList = new List<IItemHoldEffect>()
        {
            new ItemHoldEffect_Animator(rawData.ActionName),
            new ItemHoldEffect_Stat(rawData.ID, rawData.MeleeDamage, EStatType.MeleeDamage),
            new ItemHoldEffect_Stat(rawData.ID, rawData.MagicDamage, EStatType.MagicDamage),
            new ItemHoldEffect_Stat(rawData.ID, rawData.AttackSpeed, EStatType.AttackSpeed),
            new ItemHoldEffect_Stat(rawData.ID, rawData.AttackRange, EStatType.AttackRange),
        };

        var (pool, poolParent) = GetOrCreateSharedPool(rawData.PrefabPath, itemDefinition.Prefab, _itemPoolParent);
        return new ItemProfile(itemDefinition, holdEffectList, null, pool, poolParent);
    }

    public ItemProfile CreateItem(EquipmentItemRawData rawData)
    {
        var extraDescription = new List<string>()
        {
            _itemExtraDescriptionFactory.GetDescription(EItemType.Equip, EStatType.MeleeDefense, "#ffffff", rawData.MeleeDefense),
            _itemExtraDescriptionFactory.GetDescription(EItemType.Equip, EStatType.MagicDefense, "#ffffff", rawData.MagicDefense),
        };
        
        var itemDefinition = new ItemDefinition(rawData.ID, rawData.Name, rawData.Description, EItemType.Equip,
            extraDescription: extraDescription,
            isIngredient: rawData.IsIngredient,
            hasDurability: rawData.HasDurability,
            maxQuantity: rawData.MaxQuantity,
            maxDurability: rawData.MaxDuration,
            equipType: rawData.EquipType,
            iconAddressablePath: rawData.IconPath,
            prefabAddressablePath: rawData.PrefabPath);

        var holdEffectList = new List<IItemHoldEffect>()
        {
            new ItemHoldEffect_Stat(rawData.ID, rawData.MeleeDefense, EStatType.MeleeDefense),
            new ItemHoldEffect_Stat(rawData.ID, rawData.MagicDefense, EStatType.MagicDefense),
        };
        
        var (pool, poolParent) = GetOrCreateSharedPool(rawData.PrefabPath, itemDefinition.Prefab, _itemPoolParent);
        return new ItemProfile(itemDefinition, holdEffectList, null, pool, poolParent);
    }

    public ItemProfile CreateItem(UsableItemRawData rawData)
    {
        var itemDefinition = new ItemDefinition(rawData.ID, rawData.Name, rawData.Description, rawData.ItemType,
            hasDurability: rawData.HasDurability,
            maxQuantity: rawData.MaxQuantity,
            maxDurability: rawData.MaxDuration ?? 1f,
            iconAddressablePath: rawData.IconPath, 
            prefabAddressablePath: rawData.PrefabPath);

        IUseEffect useEffect = rawData.ActionName switch
        {
            "Hoe" => new UseEffect_Interact<FarmingGround>(target => target.Hoe()),
            "WateringCan" => new UseEffect_Interact<FarmingGround>(target => target.WateringCan()),
            "Seed" => new UseEffect_Interact<SeedGround>(target => target.Plant(rawData.ID)),
            "CookingPot" => new UseEffect_Interact<UnlockableObject>(target => target.Unlock()),
            _ => new UseEffectNone()
        };
        var effectList = new List<IUseEffect>() { useEffect };

        // HOld 효과 정의
        var holdAnimatorEffect = new ItemHoldEffect_Animator(rawData.ActionName);
        var holdInteractionEffect = new ItemHoldEffect_InteractionTag(rawData.InteractionTag);
        var holdEffectList = new List<IItemHoldEffect>() { holdAnimatorEffect, holdInteractionEffect };

        var (pool, poolParent) = GetOrCreateSharedPool(rawData.PrefabPath, itemDefinition.Prefab, _itemPoolParent);

        var pipeline = new ItemEffectBasePipeline(effectList);
        return new ItemProfile(itemDefinition, holdEffectList, pipeline, pool, poolParent);
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