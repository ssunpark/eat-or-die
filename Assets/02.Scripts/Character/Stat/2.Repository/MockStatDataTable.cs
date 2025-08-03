using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class MockStatDataTable
{
    public static List<CharacterStatData> GetMockData()
    {
        var statlist = new List<CharacterStatData>
        {
            new CharacterStatData(EStatType.MaxHunger, 100f),
            new CharacterStatData(EStatType.MoveSpeed, 4f),
            new CharacterStatData(EStatType.MeleeDamage, 31f),
            new CharacterStatData(EStatType.MagicDamage, 0f),
            new CharacterStatData(EStatType.TotalDamage,1f),
            new CharacterStatData(EStatType.Defense, 0f),
            new CharacterStatData(EStatType.HungerRecoveryOverTime, 0f),
            new CharacterStatData(EStatType.CritChance, 1f),
            new CharacterStatData(EStatType.AttackSpeed, 1f),
            new CharacterStatData(EStatType.JumpPower, 3f),
            new CharacterStatData(EStatType.Acceleration, 80f),
            new CharacterStatData(EStatType.HungerConsumptionOverTime, 0.1f),
            new CharacterStatData(EStatType.SprintingMultiplier, 1.5f),
            new CharacterStatData(EStatType.AttackRange, 3f)

        };

        return statlist;
    }
}
