using System.Collections.Generic;

public class ItemFactory
{
    // 주어진 데이터에 맞게 아이템 생성 후 반환
    public UseAItem CreateUseItem(UseItemRawData rawData)
    {
        var effects = new List<IUseItemEffect>();

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

        var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description);
        return new UseAItem(itemData, effects);
    }

    private IUseItemEffect CreateUseItemEffect(EUseItemEffectType type, float value, float duration)
    {
        return type switch
        {
            EUseItemEffectType.Empty => null,
            EUseItemEffectType.Hungry => new UseItemEffect_Hungry(value, duration),
            _ => null
        };
    }
}