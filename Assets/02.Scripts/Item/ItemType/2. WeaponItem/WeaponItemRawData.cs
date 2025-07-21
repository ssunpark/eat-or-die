using CsvHelper.Configuration.Attributes;

public class WeaponItemRawData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }
    
    [Name("WeaponType")]
    public EWeaponType Type { get; set; }
}