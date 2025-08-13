using System;

public interface ISkillEventHub
{ 
    void Subscribe(ISkillHandler node);
    void Unsubscribe(ISkillHandler node, SkillContext context);
    void Publish(ESkillEventType type, SkillContext context, ISkillPayload payload);
}