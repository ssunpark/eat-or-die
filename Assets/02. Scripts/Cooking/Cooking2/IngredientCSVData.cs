using CsvHelper.Configuration.Attributes;

[System.Serializable]
public class IngredientCSVData
{
    [Name("ID")]
    public int ID { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Description")]
    public string Description { get; set; }
    
    [Name("MaxStack")]
    public int MaxStack { get; set; }
    
    [Name("HungerRestore")]
    public int HungerRestore { get; set; }

    [Name("EEffectType1")]
    public string EEffectType1 { get; set; }

    [Name("EffectValue1")]
    public float EffectValue1 { get; set; }

    [Name("Duration1")]
    public float Duration1 { get; set; }

    [Name("IconPath")]
    public string IconPath { get; set; }
}