using System;
using System.Collections.Generic;

public class SkillEventFactory
{
    public ISkillHandler CreateSkillNode(SkillRawData rawData, int level = 0)
    {
        var n = rawData.NValue * level;
        
        ISkillEffect effect = rawData.ESkillEffectType switch
        {
            ESkillEffectType.HungerRestore
                => new SkillEffectAdapter<OnEatPayload>(new HungerRestore(n)),

            ESkillEffectType.FoodEffectBoost
                => new SkillEffectAdapter<OnEatPayload>(new FoodEffectBoost(n)),

            _ => throw new ArgumentOutOfRangeException(nameof(rawData.ESkillEffectType))
        };
        
        var triggers = new List<ISkillTrigger<ISkillPayload>>();
        foreach (var t in rawData.ETriggerTypes)
        {
            var v = rawData.TriggerValue ?? 0f;
            ISkillTrigger<ISkillPayload> trig = t switch
            {
                ESkillTriggerType.HungerBelowThreshold => new SkillTrigger_HungerBelowThreshold(v),
                ESkillTriggerType.HungerAboveThreshold => new SkillTrigger_HungerAboveThreshold(v),
                ESkillTriggerType.Always => new SkillTrigger_Always(),
                _ => new SkillTrigger_Always()
            };
            triggers.Add(trig);
        }

        if (triggers.Count == 0)
            triggers.Add(new SkillTrigger_Always());
        
        var eventType = rawData.EContextType; // ESkillEventType
        return eventType switch
        {
            ESkillEventType.OnEat => new SkillHandler(rawData.Id, eventType, triggers, effect),
            _ => throw new ArgumentOutOfRangeException(nameof(rawData.EContextType))
        };
    }
}