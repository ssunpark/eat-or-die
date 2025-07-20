using CsvHelper.Configuration.Attributes;

public class UsableItemRawData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }

    [Name("InteractionTag")]
    public string InteractionTag { get; set; }

    [Name("Action")]
    public EUseAction UseAction { get; set; }
}