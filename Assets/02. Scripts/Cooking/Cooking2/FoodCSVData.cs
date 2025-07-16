using CsvHelper.Configuration.Attributes;

[System.Serializable]
public class FoodCSVData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }

    [Name("ERecipeType")]
    public string ERecipeType { get; set; }

    [Name("MaxStack")]
    public int MaxStack { get; set; }

    [Name("Eatable")]
    public bool Eatable { get; set; }

    [Name("HungerRestore")]
    public int HungerRestore { get; set; }

    [Name("Ingredient1ID")]
    public string Ingredient1ID { get; set; }

    [Name("Ingredient2ID")]
    public string Ingredient2ID { get; set; }

    [Name("EffectCount")]
    public int EffectCount { get; set; }

    [Name("EffectType1")]
    public string EffectType1 { get; set; }

    [Name("EffectValue1")]
    public int EffectValue1 { get; set; }

    [Name("Duration1")]
    public float Duration1 { get; set; }

    [Name("EffectType2")]
    public string EffectType2 { get; set; }

    [Name("EffectValue2")]
    public int EffectValue2 { get; set; }

    [Name("Duration2")]
    public float Duration2 { get; set; }

    [Name("EffectType3")]
    public string EffectType3 { get; set; }

    [Name("EffectValue3")]
    public int EffectValue3 { get; set; }

    [Name("Duration3")]
    public float Duration3 { get; set; }

    [Name("IconPath(Addressable Key)")]
    public string IconPath { get; set; }
}