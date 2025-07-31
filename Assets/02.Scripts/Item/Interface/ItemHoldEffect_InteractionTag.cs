using UnityEngine;

public class ItemHoldEffect_InteractionTag : IItemHoldEffect
{
    private const string DEFAULT_TAG = "Untagged";
    
    private readonly string _interactionTag;

    public ItemHoldEffect_InteractionTag(string interactionTag)
    {
        _interactionTag = interactionTag;
    }

    public void Hold(GameObject target)
    {
        // 태그 설정
    }

    public void UnHold(GameObject target, GameObject itemObject)
    {
        // 태그 해제
    }
}