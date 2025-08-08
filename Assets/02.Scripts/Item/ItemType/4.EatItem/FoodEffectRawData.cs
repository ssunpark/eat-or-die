using CsvHelper.Configuration.Attributes;

public class FoodEffectRawData
{
    [Name("EStatType")]
    public EStatType StatType { get; set; }
    
    [Name("EStatModifierType")]
    public EStatModifierType StatModifierType { get; set; }
}