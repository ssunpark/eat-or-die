using CsvHelper.Configuration.Attributes;

public class EatEffectRawData
{
    [Name("EEffectType")]
    public EEatItemEffectType Type { get; set; }
    
    [Name("Description")]
    public string Description { get; set; }
}