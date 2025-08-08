using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ItemDefinition
{
    // 아이템 고유 ID
    public readonly int ID;

    // 아이템 종류
    public readonly EItemType Type;

    // 아이템 이름
    public readonly string Name;

    // 아이템 최대 소지 개수 (스택 가능 수)
    public readonly int MaxQuantity;

    // 아이템 최대 내구도
    public readonly float MaxDurability;

    // 요리 재료인지 여부
    public readonly bool IsIngredient;

    // 내구도를 가지는 아이템인지 여부
    public readonly bool HasDurability;

    // 공격 방식 (근거리, 원거리)
    public readonly EAttackType AttackType;

    // 장비 장착 부위
    public readonly EEquipType EquipType;

    // 아이템 설명 
    public readonly string Description;
    
    // 아이템 부가 설명 (음식 효과)
    public readonly IReadOnlyList<string> ExtraDescription;

    // 인벤토리나 UI에 사용할 아이콘 이미지
    public readonly Sprite Icon;

    // 실제 게임 내에서 사용하는 아이템 프리팹
    public readonly GameObject Prefab;

    // 발사체 키 (원거리 공격 아이템에 사용)
    private string _projectileKey;

    public string ProjectileKey
    {
        get => _projectileKey;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                _projectileKey = "DefaultProjectile";
            }
            else
            {
                _projectileKey = value;
            }
        }
    }

    public ItemDefinition(int id, string name, string description, EItemType type, 
        List<string> extraDescription = null,
        EAttackType attackType = EAttackType.MeleeWeapon,
        bool isIngredient = false,
        bool hasDurability = false,
        int maxQuantity = 1,
        float maxDurability = 1f,
        EEquipType equipType = EEquipType.None,
        string iconAddressablePath = null,
        string prefabAddressablePath = null,
        string projectileKey = null)
    {
        ID = id;
        Name = name;
        Description = description;
        Type = type;
        ExtraDescription = extraDescription ??  new List<string>();
        AttackType = attackType;
        IsIngredient = isIngredient;
        HasDurability = hasDurability;
        MaxQuantity = maxQuantity;
        MaxDurability = maxDurability;
        // MeleeDamage = meleeDamage;
        // MagicDamage = magicDamage;
        // AttackSpeed = attackSpeed;
        // Range = range;
        // MeleeDefense = meleeDefense;
        // MagicDefense = magicDefense;
        EquipType = equipType;

        // 아이콘 Addressable 경로가 비어있으면 기본값 사용
        var finalIconAddressablePath = string.IsNullOrEmpty(iconAddressablePath) ? "TestItemIcon" : iconAddressablePath;
        Icon = Addressables.LoadAssetAsync<Sprite>(finalIconAddressablePath).WaitForCompletion();

        // 프리팹 Addressable 경로가 비어있으면 기본값 사용
        var finalPrefabAddressablePath =
            string.IsNullOrEmpty(prefabAddressablePath) ? "Weapon_Staff_Prefab" : prefabAddressablePath;
        Prefab = Addressables.LoadAssetAsync<GameObject>(finalPrefabAddressablePath).WaitForCompletion();

        // 발사체 키 설정
        ProjectileKey = projectileKey ?? "DefaultProjectile";
    }
}