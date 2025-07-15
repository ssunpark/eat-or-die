using System.Collections.Generic;

public static class MockStatDataTable
{
    public static List<PlayerStatData> GetMockData()
    {
        return new List<PlayerStatData>
        {
            new PlayerStatData(EStatType.Satiety, 100f, true, 5f),
            new PlayerStatData(EStatType.MoveSpeed, 4f, true, 0.4f),
            new PlayerStatData(EStatType.Damage, 10f, true, 2.4f),
            new PlayerStatData(EStatType.Armor, 0f, true, 2f),
            new PlayerStatData(EStatType.MaxShield, 0f, false, 0f),
            new PlayerStatData(EStatType.CritChance, 1f, true, 5f),
            new PlayerStatData(EStatType.AttackSpeed, 1f, true, 0.1f),
            new PlayerStatData(EStatType.JumpPower, 5f, false, 0f),
            new PlayerStatData(EStatType.Acceleration, 80f, false, 0f),
            new PlayerStatData(EStatType.ConsumptionRate, 0.1f, true, -0.004f),
            new PlayerStatData(EStatType.SprintingMultiplier, 1.5f, true, 0.1f)

        };
    }
}
