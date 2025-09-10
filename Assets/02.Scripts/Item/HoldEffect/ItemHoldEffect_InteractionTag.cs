using UnityEngine;

public class ItemHoldEffect_InteractionTag : IItemHoldEffect
{
    private const string DEFAULT_TAG = "Untagged";
    
    private readonly string _interactionTag;

    public ItemHoldEffect_InteractionTag(string interactionTag)
    {
        _interactionTag = interactionTag ?? DEFAULT_TAG;
    }

    public void Hold(GameObject target)
    {
        // 태그 설정
        target.GetComponent<PlayerItemHolder>().InteractionTag = _interactionTag;
    }

    public void UnHold(GameObject target)
    {
        // 태그 해제
        target.GetComponent<PlayerItemHolder>().InteractionTag = DEFAULT_TAG;
    }
}