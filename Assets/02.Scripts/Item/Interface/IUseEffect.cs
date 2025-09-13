using UnityEngine;

public interface IUseEffect
{
    public void Use(GameObject target);

    public void Use(GameObject target, ItemInstance itemInstance)
    {
        Use(target);
    }
}