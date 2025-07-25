using CsvHelper.Configuration.Attributes;

public class NpcDialogue
{
    [Name("NPC")]
    public string NPCName { get; set; }
    
    [Name("NPC ID")]
    public int NPCID { get; set; }
    
    [Name("대사 ID")]
    public int DialogueID { get; set; }
    
    [Name("대사 내용")]
    public string DialogueContents { get; set; }
        
}