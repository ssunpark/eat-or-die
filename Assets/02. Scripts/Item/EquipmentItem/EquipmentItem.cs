using UnityEngine;

public class EquipmentItem : AItem, IEquipable
{
    // 장비 아이템 스텟 효과
    
    public EquipmentItem(ItemData itemData) : base(itemData)
    {
    }

    public void Equip()
    {
        Debug.Log($"장착 : {ItemData.Name}");
        // 장비 스텟 수치만큼 증가
    }

    public void Unequip()
    {
        Debug.Log($"해제 : {ItemData.Name}");
        // 장비 스텟 수치만큼 감소
    }
}