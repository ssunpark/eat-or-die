public class Skill
{
    public SkillRawData Meta { get; }
    public int Level { get; set; }
    
    public Skill(SkillRawData meta)
    {
        Meta = meta;
        Level = 0;
    }

    public bool TryUpgradeLevel()
    {
        if (Level < 5)
        {
            Level += 1;
            return true;
        }

        return false;
    }
}
