using CsvHelper.Configuration.Attributes;

public class EnemyRawData
{
    [Name("ID")]
    public int ID { get; set; }
    
    [Name("Name")]
    public string Name { get; set; }
    
    [Name("Description")]
    public string Description { get; set; }
    
    [Name("EncounterLayer")]
    public int EncounterLayer { get; set; }
    
    [Name("Hunger")]
    public float Hunger { get; set; }
    
    [Name("MoveSpeed")]
    public float MoveSpeed { get; set; }
    
    [Name("Damage")]
    public float Damage { get; set; }
    
    [Name("AttackSpeed")]
    public float AttackSpeed { get; set; }
    
    [Name("AttackRange")]
    public float AttackRange { get; set; }
    
    [Name("AttackAngle")]
    public float AttackAngle { get; set; }
    
    [Name("MeleeDefense")]
    public float MeleeDefense { get; set; }
    
    [Name("MagicDefense")]
    public float MagicDefense { get; set; }
    
    [Name("DropItem1ID")]
    public int DropItem1ID { get; set; }
    
    [Name("DropItem1Rate")]
    public float DropItem1Rate { get; set; }
    
    [Name("DropItem1Count")]
    public int DropItem1Count { get; set; }
    
    [Name("DropItem2ID")]
    public int DropItem2ID { get; set; }
    
    [Name("DropItem2Rate")]
    public float DropItem2Rate { get; set; }
    
    [Name("DropItem2Count")]
    public int DropItem2Count { get; set; }
    
    [Name("PrefabPath(Addressable)")]
    public string PrefabPath { get; set; }
}