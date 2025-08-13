public interface ISkillEffect<TPayload> where TPayload : ISkillPayload
{
    public void Execute(TPayload payload, SkillContext context);
    public void Undo(SkillContext context);
}

public interface ISkillEffect
{
    public void Execute(ISkillPayload payload, SkillContext context);
    public void Undo(SkillContext context);
}