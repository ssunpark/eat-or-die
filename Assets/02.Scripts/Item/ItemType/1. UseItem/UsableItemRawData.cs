using CsvHelper.Configuration.Attributes;

public class UsableItemRawData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }
    
    [Name("Cookable")]
    public bool Cookable { get; set; }
    
    [Name("MaxStack")]
    public int MaxQuantity { get; set; }

    [Name("InteractionTag")]
    public string InteractionTag { get; set; }

    [Name("Action")]
    public EUseAction UseAction { get; set; }

    [Name("IconPath(Addressable)")]
    public string AddressablePath { get; set; }
}