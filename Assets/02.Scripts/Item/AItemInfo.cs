// 외부 클래스에서 이벤트를 발생 시키기 위한 추상 클래스

using UnityEngine;

public abstract class AItemInfo
{
    public readonly ItemData ItemData;

    protected AItemInfo(ItemData itemData)
    {
        ItemData = itemData;
    }
    
    public abstract void Equip(GameObject player);

    public abstract void Unequip(GameObject player, GameObject itemObject = null);
}