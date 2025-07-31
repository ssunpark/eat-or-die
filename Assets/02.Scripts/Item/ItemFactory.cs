using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemFactory
{
    private readonly Transform _itemPoolParent;

    public ItemFactory(Transform itemPoolParent)
    {
        _itemPoolParent = itemPoolParent;
    }

    // 음식 아이템 효과 설명 factory
    private EatEffectManager _eatEffectManager = new();

    private Transform GetItemPoolParent(int itemID)
    {
        GameObject itemPoolParent = new GameObject($"{itemID}_Pool");
        itemPoolParent.transform.SetParent(_itemPoolParent);
        return itemPoolParent.transform;
    }

    // 주어진 데이터에 맞게 아이템 생성 후 반환
    public AItemInfo CreateItem(EatItemRawData rawData)
    {
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

        var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, true, rawData.IsIngredient,
            rawData.MaxQuantity, 1f, rawData.IconPath, "");
        return new AItemInfo(itemData, null, effectList, GetItemPoolParent(rawData.ID), extraDescription);
    }

    public AItemInfo CreateItem(WeaponItemRawData rawData)
    {
        var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, rawData.Cookable, false,
            rawData.MaxStack, rawData.MaxDuration,
            rawData.IconPath, rawData.PrefabPath);
        return new AItemInfo(itemData, null,null, GetItemPoolParent(rawData.ID));
    }

    public AItemInfo CreateItem(UsableItemRawData rawData)
    {
        var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, false, false, rawData.MaxQuantity,
            rawData.MaxDuration ?? 1f,
            rawData.AddressablePath, "");
        IUseEffect useEffect = rawData.UseAction switch
        {
            EUseAction.Plow => new UseEffectHoe(),
            EUseAction.Water => new UseEffectWateringCan(),
            EUseAction.Plant => new UseEffectSeed(rawData.ID),
        };
        var effectList = new List<IUseEffect>() {useEffect};
        return new AItemInfo(itemData, null, effectList, GetItemPoolParent(rawData.ID));
    }
}