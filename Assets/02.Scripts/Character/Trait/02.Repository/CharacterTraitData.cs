using UnityEngine;

public class CharacterTraitData
{
    // rawdata와 매칭되는 클래스
    public readonly int ID;
    public readonly ETraitType TraitType;
    public readonly string Name;
    public readonly string Description;
    public readonly EStatModifierType ModifierType;
    public readonly int MaxLevel;
    public readonly EStatType StatType;
    public readonly float ValuePerLevel;
    public readonly string ActionName;
    public readonly int ExpValue;
    public readonly int ExpPerLevel;
    public readonly string IconPath;
    public readonly Sprite Icon;

    public CharacterTraitData(int id, ETraitType traitType, string name, string description, EStatModifierType modifierType,
                              int maxLevel, EStatType statType, float valuePerLevel, string actionName,
                              int expValue, int expPerLevel, string iconPath)
    {
        ID = id;
        TraitType = traitType;
        Name = name;
        Description = description;
        ModifierType = modifierType;
        MaxLevel = maxLevel;
        StatType = statType;
        ValuePerLevel = valuePerLevel;
        ActionName = actionName;
        ExpValue = expValue;
        ExpPerLevel = expPerLevel;
        IconPath = iconPath;
    }
}