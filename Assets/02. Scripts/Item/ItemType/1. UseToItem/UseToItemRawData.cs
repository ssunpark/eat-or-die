using CsvHelper.Configuration.Attributes;

public class UseToItemRawData
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
    public EUseToAction UseToAction { get; set; }
}