using System.Collections.Generic;
public class IngredientItemData : ItemData
{
    // public bool Eatable { get; set; }
    public int HungerRestore { get; set; }
    
    public string EEffectType1 { get; set; }
    public float EffectValue1 { get; set; }
    public float Duration1 { get; set; }
    
    public IngredientItemData(IngredientCSVData csvData) : base(csvData.ID, csvData.Name, csvData.Description, csvData.MaxStack, csvData.IconPath)
    {
        HungerRestore = csvData.HungerRestore;
        EEffectType1 =  csvData.EEffectType1;
        EffectValue1 =  csvData.EffectValue1;
        Duration1 = csvData.Duration1;
    }
}
