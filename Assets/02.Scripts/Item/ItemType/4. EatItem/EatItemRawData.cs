using CsvHelper.Configuration.Attributes;

public class EatItemRawData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }

    [Name("Cookable")]
    public bool Cookable { get; set; }

    [Name("MaxStack")]
    public int MaxQuantity { get; set; }

    [Name("HungerRestore")]
    public int HungerRestore { get; set; }

    [Name("EEffectType1")]
    public EUseItemEffectType EffectType1 { get; set; }

    [Name("EffectValue1")]
    public float? Value1 { get; set; }

    [Name("Duration1")]
    public float? Duration1 { get; set; }

    [Name("EEffectType2")]
    public EUseItemEffectType EffectType2 { get; set; }

    [Name("EffectValue2")]
    public float? Value2 { get; set; }

    [Name("Duration2")]
    public float? Duration2 { get; set; }

    [Name("EEffectType3")]
    public EUseItemEffectType EffectType3 { get; set; }

    [Name("EffectValue3")]
    public float? Value3 { get; set; }

    [Name("Duration3")]
    public float? Duration3 { get; set; }

    [Name("IconPath(Addressable Key)")]
    public string IconPath { get; set; }
}