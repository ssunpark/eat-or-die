using System.Collections.Generic;
using UnityEngine;

public class EatEffectFactory
{
    private const string EFFECTTYPE_CSV_PATH = "/EatEffectTypeCSV/EatEffectType.csv";
    private Dictionary<EEatItemEffectType, string> _descriptionTemplates;

    public EatEffectFactory()
    {
        LoadDescriptions();
    }

    public IEatItemEffect CreateEatItemEffect(EEatItemEffectType type, float value, float duration)
    {
        var description = _descriptionTemplates[type];
        description = EatEffectUtils.FormatSmart(description, value, duration);
        return type switch
        {
            EEatItemEffectType.None => null,
            EEatItemEffectType.HungerInstantRecovery => new EatEffect_HungerInstantRecovery(value, description),
            EEatItemEffectType.HungerTimeRecovery => new EatEffect_HungerTimeRecovery(value, duration, description),
            EEatItemEffectType.HungerConsumeReduction => new EatEffect_HungerConsumeReduction(value, duration, description),
            EEatItemEffectType.MaxHunger => new EatEffect_MaxHunger(value, duration, description),
            EEatItemEffectType.ManaTimeRecovery => new EatEffect_ManaTimeRecovery(value, duration, description),
            EEatItemEffectType.MaxMana => new EatEffect_MaxMana(value, duration, description),
            EEatItemEffectType.MoveSpeed => new EatEffect_MoveSpeed(value, duration, description),
            EEatItemEffectType.Damage => new EatEffect_Damage(value, duration, description),
            EEatItemEffectType.MeleeDamage => new EatEffect_MeleeDamage(value, duration, description),
            EEatItemEffectType.MagicDamage => new EatEffect_MagicDamage(value, duration, description),
            EEatItemEffectType.AttackSpeed => new EatEffect_AttackSpeed(value, duration, description),
            EEatItemEffectType.Defense => new EatEffect_Defense(value, duration, description),
            EEatItemEffectType.MeleeDefense => new EatEffect_MeleeDefense(value, duration, description),
            EEatItemEffectType.MagicDefense => new EatEffect_MagicDefense(value, duration, description),
            EEatItemEffectType.BossDamage => new EatEffect_BossDamage(value, duration, description),
            EEatItemEffectType.BossDefense => new EatEffect_BossDefense(value, duration, description),
            _ => null
        };
    }



    private void LoadDescriptions()
    {
        _descriptionTemplates = new Dictionary<EEatItemEffectType, string>();
        var effectList =
            CSVLoader<EatEffectRawData>.LoadCSV($"{Application.streamingAssetsPath}{EFFECTTYPE_CSV_PATH}");
        foreach (var effect in effectList)
        {
            _descriptionTemplates.Add(effect.Type, effect.Description);
        }
    }
}