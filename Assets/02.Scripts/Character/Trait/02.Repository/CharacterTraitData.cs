public class CharacterTraitData
{
    public ETraitType TraitType;
    public int MaxLevel;
    public EStatModifierType ModifierType;
    public EStatType AffectedStat;
    public float ValuePerLevel;
    public int Level;
    public CharacterTraitData(
        ETraitType traitType,
        int maxLevel,
        EStatModifierType modifierType,
        EStatType affectedStat,
        float valuePerLevel,
        int startLevel = 0)
    {
        TraitType = traitType;
        MaxLevel = maxLevel;
        ModifierType = modifierType;
        AffectedStat = affectedStat;
        ValuePerLevel = valuePerLevel;
        Level = startLevel;
    }
}
