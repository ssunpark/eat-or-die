using CsvHelper.Configuration.Attributes;

public class SkillRawData
{
    [Name("ID")]
    public int Id { get; set; }
    
    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }

    [Name("ETraitType")]
    public ETraitType ETraitType { get; set; }

    [Name("Position")]
    public int Position { get; set; }

    [Name("EContextType")]
    public ESkillEventType EContextType { get; set; }

    [Name("ETriggerType")]
    public ESkillTriggerType ETriggerType { get; set; }

    [Name("TriggerValue")]
    public float? TriggerValue { get; set; }
    
    // 임시로 nullable
    [Name("EffectName")]
    public ESkillEffectType? ESkillEffectType { get; set; }
    
    [Name("EStatType")]
    public EStatType? EStatType { get; set; }
    
    [Name("FixedValue")]
    public float? FixedValue { get; set; }
    
    [Name("NValue")]
    public float NValue { get; set; }

    [Name("BuffDuration")]
    public float? BuffDuration { get; set; }

    [Name("IconPath(Addressable)")]
    public string IconPath { get; set; }
}