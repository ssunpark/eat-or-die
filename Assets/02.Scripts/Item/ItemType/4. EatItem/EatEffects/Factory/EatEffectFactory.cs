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
        return type switch
        {
            EEatItemEffectType.None => null,
            EEatItemEffectType.HungerInstantRecovery => new EatEffect_HungerInstantRecovery(value, description),
            EEatItemEffectType.HungerTimeRecovery => new EatEffect_HungerTimeRecovery(value, duration, description),
            EEatItemEffectType.HungerConsumeReduction => new EatEffect_HungerConsumeReduction(value, description),
            EEatItemEffectType.MaxHunger => new EatEffect_MaxHunger(value, description),
            EEatItemEffectType.ManaTimeRecovery => new EatEffect_ManaTimeRecovery(value, duration, description),
            EEatItemEffectType.MaxMana => new EatEffect_MaxMana(value, description),
            EEatItemEffectType.MoveSpeed => new EatEffect_MoveSpeed(value, description),
            EEatItemEffectType.Damage => new EatEffect_Damage(value, description),
            EEatItemEffectType.MeleeDamage => new EatEffect_MeleeDamage(value, description),
            EEatItemEffectType.MagicDamage => new EatEffect_MagicDamage(value, description),
            EEatItemEffectType.AttackSpeed => new EatEffect_AttackSpeed(value, description),
            EEatItemEffectType.Defense => new EatEffect_Defense(value, description),
            EEatItemEffectType.MeleeDefense => new EatEffect_MeleeDefense(value, description),
            EEatItemEffectType.MagicDefense => new EatEffect_MagicDefense(value, description),
            EEatItemEffectType.BossDamage => new EatEffect_BossDamage(value, description),
            EEatItemEffectType.BossDefense => new EatEffect_BossDefense(value, description),
            _ => null
        };
    }


    private void LoadDescriptions()
    {
        _descriptionTemplates = new Dictionary<EEatItemEffectType, string>();
        var effectList =
            ItemDataLoader.LoadItemRawData<EatEffectRawData>($"{Application.streamingAssetsPath}{EFFECTTYPE_CSV_PATH}");
        foreach (var effect in effectList)
        {
            _descriptionTemplates.Add(effect.Type, effect.Description);
        }
    }
}