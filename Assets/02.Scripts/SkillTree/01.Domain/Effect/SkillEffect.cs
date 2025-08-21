using UnityEngine;

// 음식 효과 배율 증가
public class SkillEffect_FoodEffectBoost : ISkillEffect<OnEatPayload>
{
    private readonly float _percent; // CSV의 NValue

    public SkillEffect_FoodEffectBoost(float percent)
    {
        _percent = percent;
    }

    public void Execute(OnEatPayload  payload, SkillContext context)
    {
        payload.Multiplier *= (1f + _percent);
    }

    public void Undo(SkillContext context) { }
}

// 배고픔 즉시 회복
public class SkillEffect_HungerRestore : ISkillEffect<OnEatPayload>
{
    private readonly float _ratio; // BaseRestore 기준 비율 (NValue)

    public SkillEffect_HungerRestore(float ratio)
    {
        _ratio = ratio;
    }

    public void Execute(OnEatPayload  payload, SkillContext context)
    {
        payload.ExtraRestore = payload.BaseRestore * _ratio;
    }

    public void Undo(SkillContext context) { }
}

// 배고픔 비율 회복
public class SkillEffect_HungerRestoreByRatio : ISkillEffect
{
    private readonly float _ratio;

    public SkillEffect_HungerRestoreByRatio(float ratio)
    {
        _ratio = ratio;
    }

    public void Execute(ISkillPayload payload, SkillContext context)
    {
        // Debug.Log($"Skill: Heal{context.MaxHunger * _ratio}");
        if (Mathf.Approximately(context.CurrentHunger, context.MaxHunger))
        {
            return;
        }
        context.Player.TryHealOrDamageFromEat(context.MaxHunger * _ratio);
    }

    public void Undo(SkillContext context) { }
}

// 확률로 음식 회복량 두 배
public class SkillEffect_HungerDoubleRestoreChance : ISkillEffect<OnEatPayload>
{
    private readonly float _chance; // 0~1 확률

    public SkillEffect_HungerDoubleRestoreChance(float chance)
    {
        _chance = chance;
    }

    public void Execute(OnEatPayload payload, SkillContext context)
    {
        if (Random.value < _chance)
            payload.Multiplier *= 2f;
    }

    public void Undo(SkillContext context) { }
}

// 스탯 변경
public class SkillEffect_StatChange : ISkillEffect
{
    private readonly float _changeValue; // 감소 비율
    private readonly EStatType _statType;
    private readonly string _source;

    public SkillEffect_StatChange(EStatType statType, float changeValue, string source)
    {
        _changeValue = changeValue;
        _statType = statType;
        _source = source;
    }

    public void Execute(ISkillPayload payload, SkillContext context)
    {
        context.Player.Stat.ApplyModifier(_statType, new StatModifier(EStatModifierType.Multiply, _changeValue, _source));
    }

    public void Undo(SkillContext context)
    {
        Debug.Log($"{_source}로 변경된 스텟 제거");
        context.Player.Stat.RemoveModifiersFrom(_source);
    }
}

// 스텟 버프
public class SkillEffect_StatBuff : ISkillEffect
{
    private readonly EStatType _statType;
    private readonly float _changeValue;
    private readonly string _source;
    private readonly float _duration;

    public SkillEffect_StatBuff(EStatType statType, float changeValue, string source, float duration)
    {
        _statType = statType;
        _changeValue = changeValue;
        _source = source;
        _duration = duration;
    }

    public void Execute(ISkillPayload payload, SkillContext context)
    {
        // Debug.Log($"{_statType}이 {EStatModifierType.Multiply}연산으로 {_changeValue}만큼 {_duration}초 동안 증가했습니다.");
        context.Player.Stat.ApplyModifier(_statType, new StatModifier(EStatModifierType.Multiply, _changeValue, _source, true, _duration));
    }

    public void Undo(SkillContext context)
    {
        Debug.Log($"{_source}로 변경된 스텟 제거");
        context.Player.Stat.RemoveModifiersFrom(_source);
    }
}

// 상태에 따른 스탯 변화
public class SkillEffect_StatChangeOnState : ISkillEffect<StatePayload>
{
    private readonly EStatType _statType;
    private readonly float _changeValue; // 감소 비율
    private readonly string _source;

    public SkillEffect_StatChangeOnState(EStatType statType, float changeValue, string source)
    {
        _statType = statType;
        _changeValue = changeValue;
        _source = source;
    }

    public void Execute(StatePayload payload, SkillContext context)
    {
        if (payload.IsEnter)
        {
            context.Player.Stat.ApplyModifier(_statType, new StatModifier(EStatModifierType.Multiply, _changeValue, _source));
        }
        else
        {
            Debug.Log($"{_source}로 변경된 스텟 제거");
            context.Player.Stat.RemoveModifiersFrom(_source);
        }
    }

    public void Undo(SkillContext context)
    {
        Debug.Log($"{_source}로 변경된 스텟 제거");
        context.Player.Stat.RemoveModifiersFrom(_source);
    }
}

// 아이템 획득
public class SkillEffect_AddItem : ISkillEffect<IItemPayload>
{
    public void Execute(IItemPayload payload, SkillContext context)
    {
        var item = ItemManager.Instance.GetItem(payload.ItemId);
        UnifiedInventoryManager.Instance.AddItem(new ItemInstance(item, payload.ItemQuantity));
    }

    public void Undo(SkillContext context)
    {
    }
}