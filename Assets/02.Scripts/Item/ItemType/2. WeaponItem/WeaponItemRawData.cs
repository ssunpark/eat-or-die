using CsvHelper.Configuration.Attributes;

public class WeaponItemRawData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }

    [Name("Cookable")]
    public bool Cookable { get; set; }

    [Name("HasDurability")]
    public bool HasDurability { get; set; }

    [Name("MaxStack")]
    public int MaxStack { get; set; }

    [Name("EWeaponType")]
    public EWeaponType Type { get; set; }

    [Name("Duration")]
    public float MaxDuration { get; set; }

    [Name("Damage")]
    public float Damage { get; set; }

    [Name("AttackSpeed")]
    public float AttackSpeed { get; set; }

    [Name("Range")]
    public float Range { get; set; }

    [Name("IconPath(Addressable)")]
    public string AddressablePath { get; set; }
}