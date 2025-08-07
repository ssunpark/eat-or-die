using UnityEngine;

public class UseEffectNone : IUseEffect
{

    public UseEffectNone()
    {
    }

    public void Use(GameObject target)
    {
        Debug.Log("[UseEffectNone] No effect applied.");
    }
}