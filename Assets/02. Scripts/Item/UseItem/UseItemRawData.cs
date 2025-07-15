using CsvHelper.Configuration.Attributes;

// 외부에서 읽어온 가공 전 데이터
public class UseItemRawData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }
    
    [Name("MaxQuantity")]
    public int MaxQuantity { get; set; }

    [Name("Addressable ID")]
    public string AddressableID { get; set; }

    [Name("EffectCount")]
    public int EffectCount { get; set; }

    [Name("EffectType1")]
    public EUseItemEffectType EffectType1 { get; set; }

    [Name("Value1")]
    public float? Value1 { get; set; }

    [Name("Duration1")]
    public float? Duration1 { get; set; }

    [Name("EffectType2")]
    public EUseItemEffectType EffectType2 { get; set; }

    [Name("Value2")]
    public float? Value2 { get; set; }

    [Name("Duration2")]
    public float? Duration2 { get; set; }

    [Name("EffectType3")]
    public EUseItemEffectType EffectType3 { get; set; }

    [Name("Value3")]
    public float? Value3 { get; set; }

    [Name("Duration3")]
    public float? Duration3 { get; set; }
}