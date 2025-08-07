using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ItemData
{
    // 아이템 고유 ID
    public readonly int ID;
    
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
    
    // 아이템 설명 (추가 가능)
    private string _description;
    public string Description => _description;
    
    // 인벤토리나 UI에 사용할 아이콘 이미지
    private Sprite _icon;
    public Sprite Icon => _icon;
    
    // 실제 게임 내에서 사용하는 아이템 프리팹
    private GameObject _prefab;
    public GameObject Prefab => _prefab;

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

    public ItemData(int id, string name, string description, bool isIngredient, bool hasDurability, int maxQuantity, float maxDurability, EAttackType attackType, string iconAddressablePath, string prefabAddressablePath, string projectileKey=null)
    {
        // TODO: 유효성 검사
        ID = id;
        Name = name;
        _description = description;
        IsIngredient = isIngredient;
        MaxQuantity = maxQuantity;
        MaxDurability = maxDurability;
        AttackType = attackType;
        HasDurability = hasDurability;

        // 아이콘 Addressable 경로가 비어있으면 기본값 사용
        var finalIconAddressablePath = iconAddressablePath == String.Empty ? "TestItemIcon" : iconAddressablePath;
        _icon = Addressables.LoadAssetAsync<Sprite>(finalIconAddressablePath).WaitForCompletion();
        
        // 프리팹 Addressable 경로가 비어있으면 기본값 사용
        var finalPrefabAddressablePath = string.IsNullOrEmpty(prefabAddressablePath) ? "Weapon_Staff_Prefab" : prefabAddressablePath;
        _prefab = Addressables.LoadAssetAsync<GameObject>(finalPrefabAddressablePath).WaitForCompletion();

        // 발사체 키 설정
        ProjectileKey = projectileKey ?? "DefaultProjectile";
    }

    // 설명 추가 메서드
    public void AddDescription(string description)
    {
        _description += $"\n{description}";
    }
}
