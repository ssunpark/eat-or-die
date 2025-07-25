using Redcode.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WeaponItemInfo : AItemInfo, IWeapon
{
    private readonly EWeaponType _type;
    private readonly float _damage;
    private readonly float _attackSpeed;
    private readonly float _range;
    
    public EWeaponType Type => _type;
    public float Damage => _damage;
    public float AttackSpeed => _attackSpeed;
    public float Range => _range;
    
    // TODO: 강화 속성은 추후에

    public WeaponItemInfo(ItemData itemData, EWeaponType weaponType, float damage, float attackSpeed, float range,
        string prefabPath, Transform poolParent) : base(itemData, prefabPath, poolParent)
    {
        _type = weaponType;
        _damage = damage;
        _attackSpeed = attackSpeed;
        _range = range;
    }
}