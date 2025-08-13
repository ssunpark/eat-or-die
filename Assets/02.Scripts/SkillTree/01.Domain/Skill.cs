using System.Collections.Generic;

public class Skill
{
    private const int MAX_LEVEL = 5;
    public readonly SkillRawData Meta;
    public int Level { get; private set; }
    private List<Skill> _parents = new List<Skill>();
    
    public Skill(SkillRawData meta)
    {
        Meta = meta;
        Level = 0;
    }

    public void AddParent(Skill parent)
    {
        _parents.Add(parent);
    }

    public bool CheckUpgradeLevel()
    {
        bool parentTest = _parents.Count > 0 ? false : true;
        foreach (var skill in _parents)
        {
            if (skill.Level >= MAX_LEVEL)
            {
                parentTest = true;
            }
        }

        if (!parentTest)
        {
            UI_Notification.Notify("상위 스킬을 먼저 5레벨 달성해야 합니다.");
            return false;
        }
        
        if (Level >= 5)
        {
            UI_Notification.Notify("최고 레벨입니다.");
            return false;
        }

        return true;
    }

    public void ResetLevel() => Level = 0;

    public void SetLevel(int level) => Level = level;
    
    public SkillDTO ToDTO() => new SkillDTO(Meta.Id, Level); 
}
