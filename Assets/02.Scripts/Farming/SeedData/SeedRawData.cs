using CsvHelper.Configuration.Attributes;

public class SeedRawData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("GrowthTime")]
    public float GrowthTime { get; set; }

    [Name("HarvestID(Item)")]
    public int? HarvestItemID { get; set; }

    [Name("PrefabPath(Addressable)")]
    public string AddressablePath { get; set; }
}