using System.Collections.Generic;

public static class MockTraitDataTable
{
    public static List<CharacterTraitData> GetMockData()
    {
        return new List<CharacterTraitData>
        {
            new CharacterTraitData(
                id: 900001,
                traitType: ETraitType.Vitality,
                name: "활력",
                description: "레벨당 최대 배고픔이 <color=#FF5555>{0}</color> 증가합니다.",
                modifierType: EStatModifierType.Add,
                maxLevel: 100,
                statType: EStatType.MaxHunger,
                valuePerLevel: 1557f,
                actionName: "KillMonster",
                expValue: 1,
                expPerLevel: 5000000,
                iconPath: "Trait_Vitality_Icon"
            ),
            new CharacterTraitData(
                id: 900002,
                traitType: ETraitType.Sprinting,
                name: "달리기",
                description: "레벨당 이동 속도가 <color=#FF5555>{0}%</color> 증가합니다.",
                modifierType: EStatModifierType.Multiply,
                maxLevel: 100,
                statType: EStatType.MoveSpeed,
                valuePerLevel: 0.1601f,
                actionName: "MovePerSecond",
                expValue: 1,
                expPerLevel: 500000,
                iconPath: "Trait_Agility_Icon"
            ),
            new CharacterTraitData(
                id: 900003,
                traitType: ETraitType.MeleeCombat,
                name: "근접 전투",
                description: "레벨당 근접 공격력이 <color=#FF5555>{0}%</color> 증가합니다.",
                modifierType: EStatModifierType.Multiply,
                maxLevel: 100,
                statType: EStatType.MeleeDamage,
                valuePerLevel: 0.88848f,
                actionName: "MeleeAttackHit",
                expValue: 1,
                expPerLevel: 20000,
                iconPath: "Trait_Melee_Icon"
            ),
            new CharacterTraitData(
                id: 900004,
                traitType: ETraitType.Magic,
                name: "마법",
                description: "레벨당 마법 공격력이 <color=#FF5555>{0}%</color> 증가합니다.",
                modifierType: EStatModifierType.Multiply,
                maxLevel: 100,
                statType: EStatType.MagicDamage,
                valuePerLevel: 0.88848f,
                actionName: "MagicAttackHit",
                expValue: 1,
                expPerLevel: 20000,
                iconPath: "Trait_Magic_Icon"
            ),
            new CharacterTraitData(
                id: 900005,
                traitType: ETraitType.Farming,
                name: "재배",
                description: "레벨당 추가 수확물을 얻을 확률이 <color=#FF5555>{0}%</color> 증가합니다.",
                modifierType: EStatModifierType.Multiply,
                maxLevel: 100,
                statType: EStatType.HarvestBonusChance,
                valuePerLevel: 0.004f,
                actionName: "HarvestPlant",
                expValue: 1,
                expPerLevel: 6600,
                iconPath: "Trait_Farming_Icon"
            ),
            new CharacterTraitData(
                id: 900006,
                traitType: ETraitType.Cooking,
                name: "요리",
                description: "레벨당 추가 요리를 얻을 확률이 <color=#FF5555>{0}%</color> 증가합니다.",
                modifierType: EStatModifierType.Multiply,
                maxLevel: 100,
                statType: EStatType.CookBonusChance,
                valuePerLevel: 0.002f,
                actionName: "RetrieveCookedFood",
                expValue: 1,
                expPerLevel: 5000,
                iconPath: "Trait_Cooking_Icon"
            )
        };
    }
}
