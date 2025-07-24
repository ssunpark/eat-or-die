using System.Collections.Generic;

public static class CharacterStatPreset
{
    public static Dictionary<EStatType, float> GetBaseStats(ECharacterType type)
    {
        return type switch
        {
            ECharacterType.Farmer => new()
            {
                { EStatType.MaxHunger, 120f },
                { EStatType.MeleeDamage, 5f },
                { EStatType.MoveSpeed, 3.5f },
                { EStatType.Defense, 0f },
                { EStatType.Acceleration, 80f},
                { EStatType.JumpPower, 3f },
                { EStatType.AttackSpeed, 1.5f },
                { EStatType.CritChance, 0.01f },
                {EStatType.SprintingMultiplier,1.5f }

            },
            ECharacterType.Warrior => new()
            {
                { EStatType.MaxHunger, 100f },
                { EStatType.MeleeDamage, 10f },
                { EStatType.MoveSpeed, 3f },
                { EStatType.Defense, 0f },
                { EStatType.Acceleration, 80f},
                { EStatType.JumpPower, 3f },
                { EStatType.AttackSpeed, 1f },
                { EStatType.CritChance, 0.01f },
                {EStatType.SprintingMultiplier,1.5f }
            },
            ECharacterType.Mage => new()
            {
                { EStatType.MaxHunger, 80f },
                { EStatType.MeleeDamage, 15f },
                { EStatType.Defense, -5f },
                { EStatType.MoveSpeed, 3.2f },
                { EStatType.Acceleration, 80f},
                { EStatType.JumpPower, 3f },
                { EStatType.AttackSpeed, 1.5f },
                { EStatType.CritChance, 0.01f },
                {EStatType.SprintingMultiplier,1.4f }
            },
            ECharacterType.Chef => new()
            {
                { EStatType.MaxHunger, 150f },
                { EStatType.MeleeDamage, 5f },
                { EStatType.MoveSpeed, 3.5f },
                { EStatType.Defense, 0f },
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
