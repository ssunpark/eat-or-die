using UnityEngine;

public class WeaponItemInfo : AItemInfo, IEquipable
{
    public readonly EWeaponType Type;
    public readonly float Damage;
    public readonly float AttackSpeed;
    public readonly float Range;
    // TODO: 강화 속성은 추후에
    
    public WeaponItemInfo(ItemData itemData, EWeaponType weaponType, float damage, float attackSpeed, float range) : base(itemData)
    {
        Type = weaponType;
        Damage = damage;
        AttackSpeed = attackSpeed;
        Range = range;
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