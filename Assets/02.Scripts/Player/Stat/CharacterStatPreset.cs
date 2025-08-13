using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class CharacterStatPreset
{
    public static Dictionary<EStatType, float> GetBaseStats(ECharacterType type)
    {
        Dictionary<EStatType, float> baseStat = type switch
        {
            ECharacterType.Farmer => new()
{
    // 이동 관련
    { EStatType.MoveSpeed, 3.5f },
    { EStatType.Acceleration, 80f },
    { EStatType.JumpPower, 3f },
    { EStatType.SprintingMultiplier, 1.5f },

    // 공격 관련
    { EStatType.TotalDamage, 1f },
    { EStatType.MeleeDamage, 5f },
    { EStatType.MagicDamage, 0f },
    { EStatType.AttackSpeed, 1.5f },
    { EStatType.AttackRange, 3f },
    { EStatType.CritChance, 0.01f },
    { EStatType.CritDamageRatio, 1.5f },
    { EStatType.BossDamage, 0f },

    // 방어 관련
    { EStatType.Defense, 0f },
    { EStatType.MeleeDefense, 0f },
    { EStatType.MagicDefense, 0f },
    { EStatType.BossDefense, 0f },

    // 포만도 관련
    { EStatType.MaxHunger, 120f },
    { EStatType.HungerConsumptionOverTime, 0.1f },
    { EStatType.HungerRecoveryOverTime, 0f },
    { EStatType.HungerConsumeReduction, 0f },

    // 마나 관련
    { EStatType.MaxMana, 0f },
    { EStatType.ManaRecoveryOverTime, 0f },

    // 기타
    { EStatType.HarvestBonusChance, 0.2f },
    { EStatType.CookBonusChance, 0.1f }
},
            ECharacterType.Warrior => new()
{
    // 이동 관련
    { EStatType.MoveSpeed, 3.0f },
    { EStatType.Acceleration, 80f },
    { EStatType.JumpPower, 3f },
    { EStatType.SprintingMultiplier, 1.4f },

    // 공격 관련
    { EStatType.TotalDamage, 1f },
    { EStatType.MeleeDamage, 15f },
    { EStatType.MagicDamage, 0f },
    { EStatType.AttackSpeed, 1.2f },
    { EStatType.AttackRange, 2.5f },
    { EStatType.CritChance, 0.05f },
    { EStatType.CritDamageRatio, 1.7f },
    { EStatType.BossDamage, 0.1f },

    // 방어 관련
    { EStatType.Defense, 10f },
    { EStatType.MeleeDefense, 8f },
    { EStatType.MagicDefense, 2f },
    { EStatType.BossDefense, 0.1f },

    // 포만도 관련
    { EStatType.MaxHunger, 100f },
    { EStatType.HungerConsumptionOverTime, 0.12f },
    { EStatType.HungerRecoveryOverTime, 0f },
    { EStatType.HungerConsumeReduction, 0.05f },

    // 마나 관련
    { EStatType.MaxMana, 0f },
    { EStatType.ManaRecoveryOverTime, 0f },

    // 기타
    { EStatType.HarvestBonusChance, 0f },
    { EStatType.CookBonusChance, 0f }
}
,
            ECharacterType.Mage => new()
{
    // 이동 관련
    { EStatType.MoveSpeed, 3.2f },
    { EStatType.Acceleration, 80f },
    { EStatType.JumpPower, 3f },
    { EStatType.SprintingMultiplier, 1.4f },

    // 공격 관련
    { EStatType.TotalDamage, 1f },
    { EStatType.MeleeDamage, 0f },
    { EStatType.MagicDamage, 20f },
    { EStatType.AttackSpeed, 0.9f },
    { EStatType.AttackRange, 5f },
    { EStatType.CritChance, 0.08f },
    { EStatType.CritDamageRatio, 2.0f },
    { EStatType.BossDamage, 0.15f },

    // 방어 관련
    { EStatType.Defense, -5f },
    { EStatType.MeleeDefense, 0f },
    { EStatType.MagicDefense, 5f },
    { EStatType.BossDefense, 0f },

    // 포만도 관련
    { EStatType.MaxHunger, 80f },
    { EStatType.HungerConsumptionOverTime, 0.1f },
    { EStatType.HungerRecoveryOverTime, 0f },
    { EStatType.HungerConsumeReduction, 0f },

    // 마나 관련
    { EStatType.MaxMana, 100f },
    { EStatType.ManaRecoveryOverTime, 2f },

    // 기타
    { EStatType.HarvestBonusChance, 0f },
    { EStatType.CookBonusChance, 0.05f }
},
            ECharacterType.Chef => new()
{
    // 이동 관련
    { EStatType.MoveSpeed, 3.5f },
    { EStatType.Acceleration, 80f },
    { EStatType.JumpPower, 3f },
    { EStatType.SprintingMultiplier, 1.5f },

    // 공격 관련
    { EStatType.TotalDamage, 1f },
    { EStatType.MeleeDamage, 7f },
    { EStatType.MagicDamage, 0f },
    { EStatType.AttackSpeed, 1.3f },
    { EStatType.AttackRange, 2.5f },
    { EStatType.CritChance, 0.02f },
    { EStatType.CritDamageRatio, 1.6f },
    { EStatType.BossDamage, 0.05f },

    // 방어 관련
    { EStatType.Defense, 3f },
    { EStatType.MeleeDefense, 2f },
    { EStatType.MagicDefense, 1f },
    { EStatType.BossDefense, 0f },

    // 포만도 관련
    { EStatType.MaxHunger, 150f },
    { EStatType.HungerConsumptionOverTime, 0.08f },
    { EStatType.HungerRecoveryOverTime, 0.5f },
    { EStatType.HungerConsumeReduction, 0.1f },

    // 마나 관련
    { EStatType.MaxMana, 0f },
    { EStatType.ManaRecoveryOverTime, 0f },

    // 기타
    { EStatType.HarvestBonusChance, 0.05f },
    { EStatType.CookBonusChance, 0.3f }
}
,
            _ => new()
        };

        // 누락된 항목 0으로 채우기
        //foreach (var stat in System.Enum.GetValues(typeof(EStatType)).Cast<EStatType>())
        //{
        //    if (!baseStat.ContainsKey(stat))
        //    {
        //        Debug.LogWarning($"{type}: base stat [{stat}] doesn't exist! Set to 0");
        //        baseStat[stat] = 0f;
        //    }
                
        //}

        return baseStat;
    }
}
