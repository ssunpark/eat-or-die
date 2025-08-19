using System.Collections.Generic;
using DarkTonic.MasterAudio;
using UnityEngine;

public class ItemEatEffectPipeline : IItemUsePipeline
{
    private readonly EatEffect_HungerInstantRecovery _hungerEffect;
    private readonly IList<IUseEffect> _useEffects;
    private readonly bool _isIngredient;

    public ItemEatEffectPipeline(EatEffect_HungerInstantRecovery hungerEffect,
        IList<IUseEffect> useEffects, bool isIngredient)
    {
        _hungerEffect = hungerEffect;
        _useEffects = useEffects;
        _isIngredient = isIngredient;
    }

    public bool Run(GameObject target)
    {
        var player = target.GetComponent<Player>();
        if (!player)
            return false;

        MasterAudio.PlaySound3DAtTransform("Eat", player.transform);
        
        var context = player.Skill.Context;

        var eatPayload = new OnEatPayload(
            eater: player.NetworkObject,
            baseRestore: _hungerEffect.Value, // 아이템 기본 회복은 각 효과가 ExtraRestore로 적는다
            hungerRatio: context.MaxHunger > 0 ? context.CurrentHunger / context.MaxHunger : 0f,
            isIngredient: _isIngredient
        );

        // 스킬 적용 (한 번만)
        player.Skill.Publish(ESkillEventType.OnEat, context, eatPayload);

        // 최종 적용 (회복)
        _hungerEffect.Use(target, eatPayload.ExtraRestore);

        // 최종 적용 (버프)
        foreach (var effect in _useEffects)
        {
            if (effect is ISkillModifiable skillModifier)
            {
                skillModifier.MultiplyValue = eatPayload.Multiplier;
            }
        }

        foreach (var useEffect in _useEffects)
        {
            useEffect.Use(target);
        }

        return true;
    }
}