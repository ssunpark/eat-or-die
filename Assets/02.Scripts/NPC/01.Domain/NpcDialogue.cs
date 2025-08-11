using CsvHelper.Configuration.Attributes;

public class NpcDialogue
{
    [Name("NPC")]
    public string NPCName { get; set; }
    
    [Name("NPC ID")]
    public int NPCID { get; set; }

    [Name("Dialogue ID")]
    public int DialogueID { get; set; }

    [Name("Dialogue Contents")]
    public string DialogueContents { get; set; }
        
}