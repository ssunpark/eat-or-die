using System;

public interface ISkillEffect<TPayload> where TPayload : ISkillPayload
{
    public void Execute(TPayload payload, SkillContext context);
}