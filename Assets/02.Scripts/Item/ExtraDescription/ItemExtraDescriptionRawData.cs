using CsvHelper.Configuration.Attributes;

public class ItemExtraDescriptionRawData
{
    [Name("EStatType")]
    public EStatType StatType { get; set; }
    
    [Name("EItemType")]
    public EItemType ItemType { get; set; }
    
    [Name("Description")]
    public string Description { get; set; }
}