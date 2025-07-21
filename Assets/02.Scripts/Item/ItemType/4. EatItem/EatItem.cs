using System.Collections.Generic;
using UnityEngine;

public struct EatItemEffectData
{
    public readonly EUseItemEffectType Type;
    public readonly float Value;
    public readonly float Duration;

    public EatItemEffectData(EUseItemEffectType type, float value, float duration)
    {
        Type = type;
        Value = value;
        Duration = duration;
    }
}

public class EatItem : AItem, IEatable, IUsable
{
    private readonly List<IEatItemEffect> _effects;

    public EatItem(ItemData itemData, List<EatItemEffectData> effectDataList) : base(itemData)
    {
        _effects = new List<IEatItemEffect>();
        foreach (var effectData in effectDataList)
        {
            var effect = CreateEatItemEffect(effectData.Type, effectData.Value,  effectData.Duration);
            _effects.Add(effect);
        }
    }
    
    private IEatItemEffect CreateEatItemEffect(EUseItemEffectType type, float value, float duration)
    {
        return type switch
        {
            EUseItemEffectType.None => null,
            EUseItemEffectType.HungerInstantRecovery => new EatEffect_HungerInstantRecovery(value),
            EUseItemEffectType.HungerTimeRecovery => new EatEffect_HungerTimeRecovery(value, duration),
            EUseItemEffectType.HungerConsumeReduction => new EatEffect_HungerConsumeReduction(value, duration),
            EUseItemEffectType.MaxHunger => new EatEffect_MaxHunger(value, duration),
            EUseItemEffectType.ManaTimeRecovery => new EatEffect_ManaTimeRecovery(value, duration),
            EUseItemEffectType.MaxMana => new EatEffect_MaxMana(value, duration),
            _ => null
        };
    }

    public void Eat()
    {
        foreach (var effect in _effects)
        {
            effect.UseEffect();
        }
    }

    public void Use(GameObject target)
    {
        // 타겟에게 효과 주도록 수정
        foreach (var effect in _effects)
        {
            effect.UseEffect();
        }
    }
}