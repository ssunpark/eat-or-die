using System.Collections.Generic;
public class FoodItemData : ItemData
{
    public bool Eatable { get; set; }
    public int HungerRestore { get; set; }
    
    public string Ingredient1ID { get; set; }
    public string Ingredient2ID { get; set; }
    
    public FoodItemData(FoodCSVData csvData) : base(csvData.ID, csvData.Name, csvData.Description, csvData.MaxStack, csvData.IconPath)
    {
        Eatable = csvData.Eatable;
        HungerRestore = csvData.HungerRestore;
        Ingredient1ID = csvData.Ingredient1ID;
        Ingredient2ID = csvData.Ingredient2ID;
    }
}
