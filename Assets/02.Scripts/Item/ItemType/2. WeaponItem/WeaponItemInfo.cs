using Redcode.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WeaponItemInfo : AItemInfo
{
    public readonly EWeaponType Type;
    public readonly float Damage;
    public readonly float AttackSpeed;
    public readonly float Range;

    private readonly Transform _poolParent;
    private Pool<Transform> _weaponPool;
    // TODO: 강화 속성은 추후에

    public WeaponItemInfo(ItemData itemData, EWeaponType weaponType, float damage, float attackSpeed, float range,
        string prefabPath, Transform poolParent) : base(itemData)
    {
        Type = weaponType;
        Damage = damage;
        AttackSpeed = attackSpeed;
        Range = range;
        _poolParent = poolParent;

        // 풀링
        GameObject plantPrefab = Addressables.LoadAssetAsync<GameObject>(prefabPath).WaitForCompletion();
        _weaponPool = Pool.Create(plantPrefab.transform, 10, poolParent.transform);
    }

    public override void Equip(GameObject player)
    {
        Debug.Log($"장착 : {ItemData.Name}");
        // 장비 스텟 수치만큼 증가
        // var weaponObject = _weaponPool.Get();
        player.GetComponent<PlayerItemHolder>().SetHoldItem(ItemData.ID);
        player.GetComponent<PlayerController>().Stat.ApplyModifier(EStatType.MeleeDamage, new StatModifier(EStatModifierType.Add, Damage, ItemData.Name));
        player.GetComponent<PlayerController>().Stat.ApplyModifier(EStatType.AttackSpeed, new StatModifier(EStatModifierType.Add, AttackSpeed, ItemData.Name));
        // player.GetComponent<StatManager>().ApplyModifier(EStatType.Range, new StatModifier(StatModifierType.Add, Range, ItemData.Name));
    }

    public override void Unequip(GameObject player, GameObject itemObject = null)
    {
        Debug.Log($"해제 : {ItemData.Name}");
        // 장비 스텟 수치만큼 감소
        player.GetComponent<StatManager>().RemoveModifiersFrom(ItemData.Name);
        
        // 풀 반환
        if (itemObject == null)
        {
            return;
        }
        
        _weaponPool.Take(itemObject.transform);
        itemObject.transform.SetParent(_poolParent);
    }
}