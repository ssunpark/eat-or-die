using UnityEngine;

public class UseEffectNone : IUseEffect
{

    public UseEffectNone()
    {
    }

    public void Use(GameObject target)
    {
        Debug.LogWarning("[UseEffectNone] No effect applied.");
    }
}