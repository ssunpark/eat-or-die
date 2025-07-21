using CsvHelper.Configuration.Attributes;

[System.Serializable]
public class RecipeCSVData
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
    
    [Name("HungerRestore")]
    public int HungerRestore { get; set; }
    
    [Name("Ingredient1ID")]
    public int Ingredient1ID { get; set; }
    [Name("Ingredient2ID")]
    public int Ingredient2ID { get; set; }
    
    [Name("EEffectType1")]
    public string EEffectType1 { get; set; }

    [Name("EffectValue1")]
    public float EffectValue1 { get; set; }

    [Name("Duration1")]
    public float Duration1 { get; set; }
    
    [Name("EEffectType2")]
    
    public string EEffectType2 { get; set; }
    [Name("EffectValue2")]
    
    public float EffectValue2 { get; set; }
    [Name("Duration2")]
    public float Duration2 { get; set; }
    
    [Name("EEffectType3")]
    public string EEffectType3 { get; set; }
    
    [Name("EffectValue3")]
    public float EffectValue3 { get; set; }
    
    [Name("Duration3")]
    public float Duration3 { get; set; }
    
    [Name("IconPath")]
    public string IconPath { get; set; }
}