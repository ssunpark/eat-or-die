// 음식 효과 배율 증가

using UnityEngine;

public class FoodEffectBoost : ISkillEffect<OnEatPayload>
{
    private readonly float _percent; // CSV의 NValue

    public FoodEffectBoost(float percent)
    {
        _percent = percent;
    }

    public void Execute(OnEatPayload  payload, SkillContext context)
    {
        payload.Multiplier *= (1f + _percent);
    }
}

// 배고픔 즉시 회복
public class HungerRestore : ISkillEffect<OnEatPayload>
{
    private readonly float _ratio; // BaseRestore 기준 비율 (NValue)

    public HungerRestore(float ratio)
    {
        _ratio = ratio;
    }

    public void Execute(OnEatPayload  payload, SkillContext context)
    {
        payload.ExtraRestore = payload.BaseRestore * _ratio;
    }
}

// 배고픔 비율 회복
public class HungerRestoreByRatio : ISkillEffect
{
    private readonly float _ratio;

    public HungerRestoreByRatio(float ratio)
    {
        _ratio = ratio;
    }

    public void Execute(ISkillPayload payload, SkillContext context)
    {
        Debug.Log($"Skill: Heal{context.MaxHunger * _ratio}");
        if (Mathf.Approximately(context.CurrentHunger, context.MaxHunger))
        {
            return;
        }
        context.Player.TryHealOrDamageFromEat(context.MaxHunger * _ratio);
    }
}

// // 확률로 음식 회복량 두 배
// public class HungerDoubleRestoreChance : ISkillEffect<OnEatPayload>
// {
//     public float Chance; // 0~1 확률
//
//     public void Execute(OnEatPayload payload, SkillContext context)
//     {
//         if (context.Random.NextFloat() < Chance)
//             payload.Multiplier *= 2f;
//     }
// }

// // 배고픔 소모 감소
// public sealed class HungerConsumeReduction : ISkillEffect<OnHungerConsumePayload>
// {
//     public float Percent; // 감소 비율
//
//     public void Execute(OnHungerConsumePayload payload, SkillContext ctx)
//     {
//         payload.ConsumeAmount *= (1f - Percent);
//     }
// }
//
// // 공격 속도 시간 버프
// public sealed class AttackSpeedTimeBuff : ISkillEffect<OnEatPayload>
// {
//     public float BuffValue;
//     public float Duration;
//
//     public void Execute(OnEatPayload payload, SkillContext ctx)
//     {
//         ctx.BuffSystem.AddBuff(new StatBuff
//         {
//             StatType = StatType.AttackSpeed,
//             Value = BuffValue,
//             Duration = Duration
//         });
//     }
// }