using UnityEngine;

public class ItemHoldEffect_Animator : IItemHoldEffect
{
    private readonly string _overrideName;

    public ItemHoldEffect_Animator(string overrideName)
    {
        _overrideName = overrideName;
    }

    public void Hold(GameObject target)
    {
        
    }

    public void UnHold(GameObject target, GameObject itemObject)
    {
        
    }
}