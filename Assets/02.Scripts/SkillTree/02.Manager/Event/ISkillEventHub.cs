using System;

public interface ISkillEventHub
{ 
    void Subscribe(IRuntimeSkill node);
    void Unsubscribe(IRuntimeSkill node);
    void Publish(ESkillEventType type, SkillContext context, ISkillPayload payload);
}