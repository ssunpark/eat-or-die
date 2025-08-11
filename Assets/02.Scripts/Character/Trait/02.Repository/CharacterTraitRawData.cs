using CsvHelper.Configuration.Attributes;

public class CharacterTraitRawData
{
    [Name("ID")]
    public int ID { get; set; }
    [Name("TraitType")]
    public ETraitType TraitType { get; set; }
    [Name("Name")]
    public string Name { get; set; }
    [Name("Description")]
    public string Description { get; set; }
    [Name("EStatModifierType")]
    public EStatModifierType ModifierType { get; set; }
    [Name("MaxLevel")]
    public int MaxLevel { get; set; }
    [Name("StatType")]
    public EStatType StatType { get; set; }
    [Name("ValuePerLevel")]
    public float ValuePerLevel { get; set; }
    [Name("ActionName")]
    public string ActionName { get; set; }
    [Name("ExpValue")]
    public int ExpValue { get; set; }
    [Name("ExpPerLevel")]
    public int ExpPerLevel { get; set; }
    [Name("IconPath(Addressable)")]
    public string IconPath { get; set; }

    public static CharacterTraitData FromRaw(CharacterTraitRawData raw)
    {
        return new CharacterTraitData(
            raw.ID,
            raw.TraitType,
            raw.Name,
            raw.Description,
            raw.ModifierType,
            raw.MaxLevel,
            raw.StatType,
            raw.ValuePerLevel,
            raw.ActionName,
            raw.ExpValue,
            raw.ExpPerLevel,
            raw.IconPath
        );
    }
}
