using System.Collections.Generic;

public static class MockStatDataTable
{
    public static List<CharacterStatData> GetMockData()
    {
        return new List<CharacterStatData>
        {
            new CharacterStatData(EStatType.MaxHunger, 100f),
            new CharacterStatData(EStatType.MoveSpeed, 4f),
            new CharacterStatData(EStatType.MeleeDamage, 10f),
            new CharacterStatData(EStatType.MagicDamage, 10f),
            new CharacterStatData(EStatType.Defense, 0f),
            new CharacterStatData(EStatType.HungerRecoveryOverTime, 0f),
            new CharacterStatData(EStatType.CritChance, 1f),
            new CharacterStatData(EStatType.AttackSpeed, 1f),
            new CharacterStatData(EStatType.JumpPower, 3f),
            new CharacterStatData(EStatType.Acceleration, 80f),
            new CharacterStatData(EStatType.HungerConsumptionOverTime, 0.1f),
            new CharacterStatData(EStatType.SprintingMultiplier, 1.5f),
            new CharacterStatData(EStatType.AttackRange, 1f)

        };
    }
}
