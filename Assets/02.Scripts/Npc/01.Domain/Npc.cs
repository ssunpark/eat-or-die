using System.Collections.Generic;
using CsvHelper.Configuration.Attributes;
using UnityEngine;
// 수현

[System.Serializable]
public class Npc
{
    [Name("ID")]
    public int ID { get; set; }
    
    [Name("Name")]
    public string Name { get; set; }
    
    [Name("Description")]
    public string Description { get; set; }
    
    [Name("UnlockFloor")]
    public int UnlockFloor { get; set; }
}
