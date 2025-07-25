using System.Collections.Generic;

public static class MockTraitDataTable
{
    public static List<CharacterTraitData> GetMockData()
    {
        return new List<CharacterTraitData>
        {
            new CharacterTraitData(
                ETraitType.Vitality, 100, EStatModifierType.Add,
                EStatType.MaxHunger, 1f),

            new CharacterTraitData(
                ETraitType.Sprinting, 100, EStatModifierType.Multiply,
                EStatType.MoveSpeed, 0.001f),

            new CharacterTraitData(
                ETraitType.MeleeCombat, 100, EStatModifierType.Multiply,
                EStatType.MeleeDamage, 0.005f),

            new CharacterTraitData(
                ETraitType.Magic, 100, EStatModifierType.Multiply,
                EStatType.MagicDamage, 0.005f),

            new CharacterTraitData(
                ETraitType.Farming, 100, EStatModifierType.Multiply,
                EStatType.HarvestBonusChance, 0.004f),

            new CharacterTraitData(
                ETraitType.Cooking, 100, EStatModifierType.Multiply,
                EStatType.CookBonusChance, 0.002f),
        };
    }
}
