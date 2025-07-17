using System.Collections.Generic;

public class ItemFactory
{
    // 주어진 데이터에 맞게 아이템 생성 후 반환
    public EatItem CreateUseItem(EatItemRawData rawData)
    {
        var effects = new List<IEatItemEffect>();

        var rawEffects = new (EUseItemEffectType type, float? value, float? duration)[]
        {
            (rawData.EffectType1, rawData.Value1, rawData.Duration1),
            (rawData.EffectType2, rawData.Value2, rawData.Duration2),
            (rawData.EffectType3, rawData.Value3, rawData.Duration3),
        };

        foreach (var (type, value, duration) in rawEffects)
        {
            if (type == EUseItemEffectType.Empty)
                continue;

            var effect = CreateUseItemEffect(type, value ?? 0f, duration ?? 0f);
            if (effect != null)
                effects.Add(effect);
        }

        var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, rawData.MaxQuantity, "");
        return new EatItem(itemData, effects);
    }

    private IEatItemEffect CreateUseItemEffect(EUseItemEffectType type, float value, float duration)
    {
        return type switch
        {
            EUseItemEffectType.Empty => null,
            EUseItemEffectType.Hungry => new EatEffect_Hungry(value),
            _ => null
        };
    }

    public EquipmentItem CreateEquipmentItem(EquipmentItemRawData rawData)
    {
        var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, 1, "");
        return new EquipmentItem(itemData);
    }
    
    public WeaponItem CreateWeaponItem(WeaponItemRawData rawData)
    {
        var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, 1, "");
        return new WeaponItem(itemData, rawData.Type);
    }
}