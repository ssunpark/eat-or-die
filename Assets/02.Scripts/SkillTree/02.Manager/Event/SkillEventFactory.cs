public class SkillEventFactory
{
    public IRuntimeSkill CreateSkillNode(SkillRawData rawData, int level = 0)
    {
        var n = rawData.NValue * level;
        
        var skillEffect = rawData.ESkillEffectType switch
        {
            ESkillEffectType.HungerRestore => new HungerRestore(n),
        };
        
        var skill = rawData.EContextType switch
        {
            ESkillEventType.OnEat => new SkillEvent<OnEatPayload>(rawData.Id, ESkillEventType.OnEat, skillEffect),
        };

        return skill;
    }
}