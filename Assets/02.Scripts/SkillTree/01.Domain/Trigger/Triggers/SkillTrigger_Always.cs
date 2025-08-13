public class SkillTrigger_Always : ISkillTrigger<ISkillPayload>
{
    public bool CanTrigger(ISkillPayload payload, SkillContext context)
    {
        return true;
    }
}