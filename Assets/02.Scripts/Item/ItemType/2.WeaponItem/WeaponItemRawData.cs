using CsvHelper.Configuration.Attributes;

public class WeaponItemRawData
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
    
    [Name("EAttackType")]
    public EAttackType AttackType { get; set; }

    [Name("MaxStack")]
    public int MaxStack { get; set; }

    [Name("ActionName")]
    public string ActionName { get; set; }

    [Name("Duration")]
    public float MaxDuration { get; set; }

    [Name("MeleeDamage")]
    public float MeleeDamage { get; set; }
    
    [Name("MagicDamage")]
    public float MagicDamage { get; set; }

    [Name("AttackSpeed")]
    public float AttackSpeed { get; set; }

    [Name("Range")]
    public float Range { get; set; }

    [Name("IconPath(Addressable)")]
    public string IconPath { get; set; }
    
    [Name("PrefabPath(Addressable)")]
    public string PrefabPath { get; set; }
}