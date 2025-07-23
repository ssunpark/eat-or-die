using UnityEngine;

public class UsableItem : AItem, IUsable, IEquipable
{
    private readonly IUseAction _useAction;
    private readonly string _interactionTag;
    
    public UsableItem(ItemData itemData, string interactionTag, IUseAction useAction) : base(itemData)
    {
        _interactionTag = interactionTag;
        _useAction = useAction;
    }
    
    public void Use(GameObject target)
    {
        // target에 도구 사용
        _useAction.UseTool(target);
    }

    public void Equip(GameObject player)
    {
        // 장착하면 상호작용 할 태그 수정
        player.GetComponent<PlayerInteractions>().OnUnequipped();
        player.GetComponent<PlayerInteractions>().OnEquipped(ItemData.ID, _interactionTag);
        
        
        player.GetComponent<PlayerItemHolder>().SetHoldItem(ItemData.ID);
    }

    public void Unequip(GameObject player)
    {
        // 해제하면 상호작용 할 태그 수정
        player.GetComponent<PlayerInteractions>().OnUnequipped();

        player.GetComponent<PlayerItemHolder>().SetHoldItem(0);
    }
}