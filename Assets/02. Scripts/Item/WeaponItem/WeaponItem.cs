using UnityEngine;

public class WeaponItem : AItem, IEquipable
{
    public readonly EWeaponType _type;
    // TODO: 강화 속성은 추후에
    
    public WeaponItem(ItemData itemData, EWeaponType weaponType) : base(itemData)
    {
        _type = weaponType;
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