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
    public EatItemInfo CreateEatItem(EatItemRawData rawData)
    {
        var effectList = new List<IEatItemEffect>();

        // 기본 배고픔
        IEatItemEffect hungerEffect = new EatEffect_HungerInstantRecovery(rawData.HungerRestore);
        effectList.Add(hungerEffect);

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
                var effect = CreateEffect(statType, value ?? 0f, duration ?? 0f);
                effectList.Add(effect);
            }
        }

        var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, true, rawData.IsIngredient,
            rawData.MaxQuantity, 1f, rawData.IconPath, "");
        return new EatItemInfo(itemData, GetItemPoolParent(rawData.ID), effectList);
    }

    private IEatItemEffect CreateEffect(EStatType statType, float value, float duration)
    {
        var type = _eatEffectManager.GetStatModifierType(statType);
        var desc = _eatEffectManager.GetDescription(statType, value, duration);
        return new EatEffect_StatModifier(statType, value, duration, type, desc);
    }

    // public EquipmentItem CreateEquipmentItem(EquipmentItemRawData rawData)
    // {
    //     var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, 1, "");
    //     return new EquipmentItem(itemData);
    // }

    public WeaponItemInfo CreateWeaponItem(WeaponItemRawData rawData)
    {
        var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, rawData.Cookable, false,
            rawData.MaxStack, rawData.MaxDuration,
            rawData.IconPath, rawData.PrefabPath);
        return new WeaponItemInfo(itemData, GetItemPoolParent(rawData.ID), rawData.Type, rawData.Damage,
            rawData.AttackSpeed, rawData.Range);
    }

    public UsableItemInfo CreateUsableItem(UsableItemRawData rawData)
    {
        var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, false, false, rawData.MaxQuantity,
            rawData.MaxDuration ?? 1f,
            rawData.AddressablePath, "");
        IUseAction useAction = rawData.UseAction switch
        {
            EUseAction.Plow => new UseActionHoe(),
            EUseAction.Water => new UseActionWateringCan(),
            EUseAction.Plant => new UseActionSeed(rawData.ID),
        };
        return new UsableItemInfo(itemData, GetItemPoolParent(rawData.ID), rawData.InteractionTag, useAction);
    }
}