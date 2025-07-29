using UnityEngine;

public interface IUsable
{
    public string InteractionTag { get; }
    public void Use(GameObject target);
}