using System.Collections.Generic;

public class SkillHandlerFactory
{
    public ISkillHandler CreateSkillNode(SkillRawData rawData, int level = 0)
    {
        var n = rawData.NValue * level;
        
        ISkillEffect effect = rawData.ESkillEffectType switch
        {
            ESkillEffectType.HungerRestore
                => new SkillEffectAdapter<OnEatPayload>(new SkillEffect_HungerRestore(n)),
            ESkillEffectType.FoodEffectBoost
                => new SkillEffectAdapter<OnEatPayload>(new SkillEffect_FoodEffectBoost(n)),
            ESkillEffectType.HungerRestoreByMaxHunger
                => new SkillEffect_HungerRestoreByRatio(n),
            ESkillEffectType.HungerDoubleRestoreChance
                => new SkillEffectAdapter<OnEatPayload>(new SkillEffect_HungerRestore(1f)),
            ESkillEffectType.StatChange
                => new SkillEffect_StatChange(rawData.EStatType ?? EStatType.MaxHunger, n, $"Skill_{rawData.Id}"),
            ESkillEffectType.StatBuff
                => new SkillEffect_StatBuff(rawData.EStatType ?? EStatType.MaxHunger, n, $"Skill_{rawData.Id}", rawData.BuffDuration ?? 0f),
            _ => null
        };

        if (effect == null)
        {
            return null;
        }
        
        var triggers = new List<ISkillTrigger<ISkillPayload>>();
        foreach (var triggerType in rawData.ETriggerTypes)
        {
            var triggerValue = rawData.TriggerValue ?? 0f;
            ISkillTrigger<ISkillPayload> trigger = triggerType switch
            {
                ESkillTriggerType.HungerBelowThreshold => new SkillTrigger_HungerThreshold(triggerValue, false),
                ESkillTriggerType.HungerAboveThreshold => new SkillTrigger_HungerThreshold(triggerValue, true),
                ESkillTriggerType.StateTime => new SkillTrigger_StateThreshold(triggerValue, rawData.EPlayerState ?? EPlayerState.Idle),
                ESkillTriggerType.EveryOneSecond => new SkillTrigger_EveryOneSecond(),
                ESkillTriggerType.OnNChance => new SkillTrigger_OnNChance(n),
                ESkillTriggerType.IsState => new SkillTrigger_IsState(rawData.EPlayerState ?? EPlayerState.Idle),
                ESkillTriggerType.Always => new SkillTrigger_Always(),
                _ => new SkillTrigger_Always()
            };
            triggers.Add(trigger);
        }

        if (triggers.Count == 0)
            triggers.Add(new SkillTrigger_Always());

        return new SkillHandler(rawData.Id, rawData.EEventType, triggers, effect);
    }
}