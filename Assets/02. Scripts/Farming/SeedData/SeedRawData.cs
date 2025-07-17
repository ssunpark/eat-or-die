using CsvHelper.Configuration.Attributes;

public class SeedRawData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }

    [Name("MaxStack")]
    public int MaxStack { get; set; }

    [Name("GrowthTime")]
    public float GrowthTime { get; set; }

    [Name("HarvestID(Item)")]
    public int HarvestItemID { get; set; }

    [Name("IconPath(Addressable)")]
    public string IconPath { get; set; }
}