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

    [Name("IconPath(Addressable Key)")]
    public string IconPath { get; set; }
}