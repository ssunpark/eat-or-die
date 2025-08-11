using System;
using UnityEngine;

public class UseEffect_Interact<T> : IUseEffect where T : Component
{
    private readonly Action<T> _effectAction;

    public UseEffect_Interact(Action<T> effectAction)
    {
        _effectAction = effectAction;
    }

    public void Use(GameObject target)
    {
        if (target.TryGetComponent(out T component))
        {
            _effectAction?.Invoke(component);
        }
    }
}