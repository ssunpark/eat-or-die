using System.Collections.Generic;
using UnityEngine;

public class ItemEffectBasePipeline : IItemUsePipeline
{
    private IList<IUseEffect> _useEffects;

    public ItemEffectBasePipeline(IList<IUseEffect> useEffects)
    {
        _useEffects = useEffects;
    }

    public bool Run(GameObject target)
    {
        foreach (var useEffect in _useEffects)
        {
            useEffect.Use(target);
        }
        
        return true;
    }
}