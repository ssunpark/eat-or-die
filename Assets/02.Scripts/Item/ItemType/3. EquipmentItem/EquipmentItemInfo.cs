using UnityEngine;

public class EquipmentItemInfo : AItemInfo, IEquipable
{
    // 장비 아이템 스텟 효과
    
    public EquipmentItemInfo(ItemData itemData) : base(itemData)
    {
    }

    public void Equip(GameObject player)
    {
        Debug.Log($"장착 : {ItemData.Name}");
        // 장비 스텟 수치만큼 증가
    }

    public void Unequip(GameObject player, GameObject itemObject = null)
    {
        Debug.Log($"해제 : {ItemData.Name}");
        // 장비 스텟 수치만큼 감소
    }
}