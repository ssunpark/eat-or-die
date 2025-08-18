using Fusion;
using UnityEngine;

public class EatEffect_StatModifier : IUseEffect, ISkillModifiable
{
    private const string Food = "Food";
    
    private readonly float _value;
    private readonly float _duration;
    private readonly EStatType _statType;
    private readonly EStatModifierType _modifierType;
    public float MultiplyValue { get; set; }

    private int _foodID;

    public EatEffect_StatModifier(int foodID)
    {
        _foodID = foodID;
    }
    
    public void Use(GameObject target)
    {
        var fsm = target.GetComponent<PlayerFSM>();
        if (fsm == null) return;

        float finalValue = _value * MultiplyValue;

        fsm.RPC_RequestUseFood(_foodID,fsm.Object);

        MultiplyValue = 1f;
    }

    public void UseOnTarget(PlayerFSM myFsm, NetworkObject target, int foodId)
    {
        if (myFsm == null || !myFsm.HasInputAuthority) return;
        myFsm.RPC_RequestUseFoodOnTarget(foodId, target);
    }
}