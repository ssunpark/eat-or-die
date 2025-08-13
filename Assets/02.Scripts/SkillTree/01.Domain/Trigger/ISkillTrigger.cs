public interface ISkillTrigger<TPayload> where TPayload : ISkillPayload
{
    public bool CanTrigger(TPayload payload, SkillContext context);
}