public class Skill
{
    public SkillRawData Meta { get; }
    public int Level { get; set; }
    
    public Skill(SkillRawData meta, int level)
    {
        Meta = meta;
        Level = level;
    }

    public void UpgradeLevel(int level)
    {
        
    }
}
