using UnityEngine;

public interface IEatItemEffect
{
    public string Description { get; }
    
    public void UseEffect(GameObject target);
}