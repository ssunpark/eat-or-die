using CsvHelper.Configuration.Attributes;

[System.Serializable]
public class Recipe
{
    [Name("ID")]
    public int ID { get; set; }
    
    [Name("Ingredient 1 ID")]
    public int Ingredient1ID { get; set; }
    [Name("Ingredient 2 ID")]
    public int? Ingredient2ID { get; set; } // null값도 받아오기 위해 ? 추가
    
    [Name("Result ID")]
    public int ResultID { get; set; }
    
    [Name("Recipe Name")]
    public string Name { get; set; }
}