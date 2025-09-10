using CsvHelper.Configuration.Attributes;

public class ExtraItemRawData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }

    [Name("HasDurability")]
    public bool HasDurability { get; set; }

    [Name("MaxStack")]
    public int MaxStack { get; set; }

    [Name("Duration")]
    public float? Duration { get; set; }

    [Name("IconPath(Addressable)")]
    public string IconPath { get; set; }

    [Name("PrefabPath(Addressable)")]
    public string PrefabPath { get; set; }

    [Name("ExtraInfo")]
    public string ExtraInfo { get; set; }
    
    [Ignore]
    public EItemType ItemType { get; set; }
}