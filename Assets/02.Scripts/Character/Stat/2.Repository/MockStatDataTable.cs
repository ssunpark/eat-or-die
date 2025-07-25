using System.Collections.Generic;

public static class MockStatDataTable
{
    public static List<CharacterStatData> GetMockData()
    {
        return new List<CharacterStatData>
        {
            new CharacterStatData(EStatType.MaxHunger, 100f, true, 5f),
            new CharacterStatData(EStatType.MoveSpeed, 4f, true, 0.4f),
            new CharacterStatData(EStatType.MeleeDamage, 10f, true, 2.4f),
            new CharacterStatData(EStatType.MagicDamage, 10f, true, 2.4f),
            new CharacterStatData(EStatType.Defense, 0f, true, 2f),
            new CharacterStatData(EStatType.HungerRecoveryOverTime, 0f, false, 0f),
            new CharacterStatData(EStatType.CritChance, 1f, true, 5f),
            new CharacterStatData(EStatType.AttackSpeed, 1f, true, 0.1f),
            new CharacterStatData(EStatType.JumpPower, 3f, false, 0f),
            new CharacterStatData(EStatType.Acceleration, 80f, false, 0f),
            new CharacterStatData(EStatType.ConsumptionOverTime, 0.1f, true, -0.004f),
            new CharacterStatData(EStatType.SprintingMultiplier, 1.5f, true, 0.1f),
            new CharacterStatData(EStatType.AttackRange, 1f, false, 0f)

        };
    }
}
