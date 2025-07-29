using CsvHelper.Configuration.Attributes;
using UnityEngine;
// 수현
public class CraftRecipe
{
    [Name("ID")]
    public int ID { get; set; }
    
    [Name("CraftMaterial1ID")]
    public int CraftMaterial1ID { get; set; }
    
    [Name("CraftMaterial1Count")]
    public int CraftMaterial1Count  { get; set; }
    
    [Name("CraftMaterial2ID")]
    public int CraftMaterial2ID  { get; set; }
    
    [Name("CraftMaterial2Count")]
    public int CraftMaterial2Count  { get; set; }
    
    [Name("CraftResultID")]
    public int CraftResultID  { get; set; }
    
    [Name("CraftRecipeName")]
    public string CraftRecipeName  { get; set; }
}
