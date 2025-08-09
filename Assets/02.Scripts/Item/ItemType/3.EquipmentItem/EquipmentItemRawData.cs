using CsvHelper.Configuration.Attributes;

public class EquipmentItemRawData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }

    [Name("IsIngredient")]
    public bool IsIngredient { get; set; }

    [Name("HasDurability")]
    public bool HasDurability { get; set; }

    [Name("MaxStack")]
    public int MaxQuantity { get; set; }

    [Name("EEquipType")]
    public EEquipType EquipType { get; set; }
    
    [Name("Durability")]
    public float MaxDuration { get; set; }
    
    [Name("Melee Defense")]
    public float MeleeDefense { get; set; }

    [Name("Magic Defense")]
    public float MagicDefense { get; set; }
    
    [Name("IconPath(Addressable)")]
    public string IconPath { get; set; }
    
    [Name("PrefabPath(Addressable)")]
    public string PrefabPath { get; set; }
}