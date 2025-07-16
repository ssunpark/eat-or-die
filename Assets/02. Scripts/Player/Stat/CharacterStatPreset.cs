using System.Collections.Generic;

public static class CharacterStatPreset
{
    public static Dictionary<EStatType, float> GetBaseStats(CharacterClassType type)
    {
        return type switch
        {
            CharacterClassType.Farmer => new()
            {
                { EStatType.Satiety, 120f },
                { EStatType.Damage, 5f },
                { EStatType.MoveSpeed, 3.5f },
                { EStatType.Armor, 0f },
                { EStatType.Acceleration, 80f},
                { EStatType.JumpPower, 3f },
                { EStatType.AttackSpeed, 1.2f },
                { EStatType.CritChance, 0.01f },
                {EStatType.SprintingMultiplier,1.5f }

            },
            CharacterClassType.Warrior => new()
            {
                { EStatType.Satiety, 100f },
                { EStatType.Damage, 10f },
                { EStatType.MoveSpeed, 3f },
                { EStatType.Armor, 0f },
                { EStatType.Acceleration, 80f},
                { EStatType.JumpPower, 3f },
                { EStatType.AttackSpeed, 1f },
                { EStatType.CritChance, 0.01f },
                {EStatType.SprintingMultiplier,1.5f }
            },
            CharacterClassType.Mage => new()
            {
                { EStatType.Satiety, 80f },
                { EStatType.Damage, 15f },
                { EStatType.Armor, -5f },
                { EStatType.MoveSpeed, 3.2f },
                { EStatType.Acceleration, 80f},
                { EStatType.JumpPower, 3f },
                { EStatType.AttackSpeed, 1.5f },
                { EStatType.CritChance, 0.01f },
                {EStatType.SprintingMultiplier,1.4f }
            },
            CharacterClassType.Chef => new()
            {
                { EStatType.Satiety, 150f },
                { EStatType.Damage, 5f },
                { EStatType.MoveSpeed, 3.5f },
                { EStatType.Armor, 0f },
                { EStatType.Acceleration, 80f},
                { EStatType.JumpPower, 3f },
                { EStatType.AttackSpeed, 1.3f },
                { EStatType.CritChance, 0.01f },
                {EStatType.SprintingMultiplier,1.5f }
            },
            _ => new Dictionary<EStatType, float>() // 기본 빈값
        };
    }
}
