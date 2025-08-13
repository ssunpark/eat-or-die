public class SkillTrigger_IsMad : ISkillTrigger<ISkillPayload>
{
    public bool CanTrigger(ISkillPayload payload, SkillContext context)
    {
        return context.IsBerserk;
    }
}