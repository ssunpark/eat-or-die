using CsvHelper.Configuration.Attributes;

public class EatEffectRawData
{
    [Name("EStatType")]
    public EStatType Type { get; set; }
    
    [Name("EStatModifierType")]
    public EStatModifierType StatModifierType { get; set; }
    
    [Name("Description")]
    public string Description { get; set; }
}