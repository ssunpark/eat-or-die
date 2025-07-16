using System.Collections.Generic;

public static class MockStatDataTable
{
    public static List<CharacterStatData> GetMockData()
    {
        return new List<CharacterStatData>
        {
            new CharacterStatData(EStatType.MaxSatiety, 100f, true, 5f),
            new CharacterStatData(EStatType.MoveSpeed, 4f, true, 0.4f),
            new CharacterStatData(EStatType.Damage, 10f, true, 2.4f),
            new CharacterStatData(EStatType.Armor, 0f, true, 2f),
            new CharacterStatData(EStatType.MaxShield, 0f, false, 0f),
            new CharacterStatData(EStatType.CritChance, 1f, true, 5f),
            new CharacterStatData(EStatType.AttackSpeed, 1f, true, 0.1f),
            new CharacterStatData(EStatType.JumpPower, 3f, false, 0f),
            new CharacterStatData(EStatType.Acceleration, 80f, false, 0f),
            new CharacterStatData(EStatType.ConsumptionRate, 0.1f, true, -0.004f),
            new CharacterStatData(EStatType.SprintingMultiplier, 1.5f, true, 0.1f)

        };
    }
}
