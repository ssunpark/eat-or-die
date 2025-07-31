using UnityEngine;

public class ItemHoldEffect_Animator : IItemHoldEffect
{
    private const string DEFAULT_OVERRIDE = "Unarmed";
    private readonly string _overrideName;

    public ItemHoldEffect_Animator(string overrideName)
    {
        _overrideName = overrideName;
    }

    public void Hold(GameObject target)
    {
        target.GetComponent<PlayerItemHolder>().ApplyAnimatorOverride(_overrideName);
    }

    public void UnHold(GameObject target)
    {
        target.GetComponent<PlayerItemHolder>().ApplyAnimatorOverride(DEFAULT_OVERRIDE);
    }
}