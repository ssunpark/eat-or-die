using UnityEngine;

public class WeaponItemInfo : AItemInfo, IEquipable
{
    public readonly EWeaponType _type;
    public readonly float Damage;
    // TODO: 강화 속성은 추후에
    
    public WeaponItemInfo(ItemData itemData, EWeaponType weaponType) : base(itemData)
    {
        _type = weaponType;
    }

    public void Equip(GameObject player)
    {
        Debug.Log($"장착 : {ItemData.Name}");
        // 장비 스텟 수치만큼 증가
    }

    public void Unequip(GameObject player)
    {
        Debug.Log($"해제 : {ItemData.Name}");
        // 장비 스텟 수치만큼 감소
    }
}