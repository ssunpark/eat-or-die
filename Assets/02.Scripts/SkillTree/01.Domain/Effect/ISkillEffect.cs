public interface ISkillEffect<TPayload> where TPayload : ISkillPayload
{
    void Execute(TPayload payload, SkillContext context);
}

public interface ISkillEffect
{
    void Execute(ISkillPayload payload, SkillContext context);
}