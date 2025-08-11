// 음식 효과 배율 증가
public class FoodEffectBoost : ISkillEffect<OnEatPayload>
{
    public float Percent; // CSV의 NValue

    public void Execute(OnEatPayload payload, SkillContext context)
    {
        payload.Multiplier *= (1f + Percent);
    }
}

// 배고픔 즉시 회복
public class HungerRestore : ISkillEffect<OnEatPayload>
{
    public float Ratio; // BaseRestore 기준 비율 (NValue)

    public HungerRestore(float ratio)
    {
        Ratio = ratio;
    }

    public void Execute(OnEatPayload payload, SkillContext context)
    {
        payload.ExtraRestore += payload.BaseRestore * Ratio;
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