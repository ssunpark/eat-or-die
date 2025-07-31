using UnityEngine;

public interface IItemHoldEffect
{
    public void Hold(GameObject target);
    
    public void UnHold(GameObject target, GameObject itemObject);
}