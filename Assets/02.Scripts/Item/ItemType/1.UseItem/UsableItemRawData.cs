using System;
using CsvHelper.Configuration.Attributes;

public class UsableItemRawData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }
    
    [Ignore]
    public EItemType ItemType { get; set; }
    
    [Name("HasDurability")]
    public bool HasDurability { get; set; }

    [Name("MaxStack")]
    public int MaxQuantity { get; set; }

    [Name("Duration")]
    public float? MaxDuration { get; set; }

    [Name("InteractionTag")]
    public string InteractionTag { get; set; }

    [Name("ActionName")]
    public string ActionName { get; set; }

    [Name("IconPath(Addressable)")]
    public string IconPath { get; set; }
    
    [Name("PrefabPath(Addressable)")]
    public string PrefabPath { get; set; }
}