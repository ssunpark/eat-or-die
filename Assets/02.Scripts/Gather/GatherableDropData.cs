using CsvHelper.Configuration.Attributes;

public class GatherableDropData
{
    [Name("GatherableID")] public int GatherableID { get; set; }
    [Name("ItemID")] public int ItemID { get; set; }
    [Name("MinCount")] public int MinCount { get; set; }
    [Name("MaxCount")] public int MaxCount { get; set; }
}