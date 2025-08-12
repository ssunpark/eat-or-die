using System.Collections.Generic;

public static class StatNameLocalization
{
    private static readonly Dictionary<EStatType, string> _names = new()
    {
        { EStatType.MoveSpeed, "이동 속도" },
        { EStatType.Acceleration, "가속도" },
        { EStatType.JumpPower, "점프력" },
        { EStatType.SprintingMultiplier, "전력 질주 배율" },

        { EStatType.TotalDamage, "총 피해량" },
        { EStatType.MeleeDamage, "근접 피해" },
        { EStatType.MagicDamage, "마법 피해" },
        { EStatType.AttackSpeed, "공격 속도" },
        { EStatType.AttackRange, "공격 범위" },
        { EStatType.CritChance, "치명타 확률" },
        { EStatType.CritDamageRatio, "치명타 피해 배율" },
        { EStatType.BossDamage, "보스 피해" },

        { EStatType.Defense, "방어력" },
        { EStatType.MeleeDefense, "근접 방어력" },
        { EStatType.MagicDefense, "마법 방어력" },
        { EStatType.BossDefense, "보스 방어력" },

        { EStatType.MaxHunger, "최대 포만도" },
        { EStatType.HungerConsumptionOverTime, "포만도 소모" },
        { EStatType.HungerRecoveryOverTime, "포만도 회복" },
        { EStatType.HungerConsumeReduction, "포만도 소모 감소" },

        { EStatType.MaxMana, "최대 마나" },
        { EStatType.ManaRecoveryOverTime, "마나 회복" },

        { EStatType.HarvestBonusChance, "수확 보너스 확률" },
        { EStatType.CookBonusChance, "요리 보너스 확률" },
        {EStatType.EvadeChance, "회피 확률" }
    };

    public static string Get(EStatType type)
    {
        return _names.TryGetValue(type, out var name) ? name : type.ToString();
    }
}
