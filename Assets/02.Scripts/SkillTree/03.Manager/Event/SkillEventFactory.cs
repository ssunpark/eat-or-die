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
            
            ESkillEffectType.HungerRestoreByMaxHunger
                => new HungerRestoreByRatio(n),

            _ => null
        };

        if (effect == null)
        {
            return null;
        }
        
        var triggers = new List<ISkillTrigger<ISkillPayload>>();
        foreach (var t in rawData.ETriggerTypes)
        {
            var v = rawData.TriggerValue ?? 0f;
            ISkillTrigger<ISkillPayload> trig = t switch
            {
                ESkillTriggerType.HungerBelowThreshold => new SkillTrigger_HungerThreshold(v, false),
                ESkillTriggerType.HungerAboveThreshold => new SkillTrigger_HungerThreshold(v, true),
                ESkillTriggerType.WhileStationary => new SkillTrigger_IdleThreshold(v),
                ESkillTriggerType.EveryOneSecond => new SkillTrigger_EveryOneSecond(),
                ESkillTriggerType.Always => new SkillTrigger_Always(),
                _ => new SkillTrigger_Always()
            };
            triggers.Add(trig);
        }

        if (triggers.Count == 0)
            triggers.Add(new SkillTrigger_Always());

        return new SkillHandler(rawData.Id, rawData.EEventType, triggers, effect);
    }
}