using System;
using UnityEngine;

// 외부에서 보이는 값이 변하는 아이템 실질적인 객체
// 내구도, 갯수 유형으로 나뉨
public class ItemInstance
{
    public readonly ItemProfile ItemProfile;
    public readonly int MaxQuantity;
    public readonly float MaxDurability;

    public int ID => ItemProfile.ItemDefinition.ID;

    private int _quantity;
    public int Quantity => _quantity;

    private float _durability;
    public float Durability => _durability;

    // 추가적인 아이템 정보
    private string _extraInfo;
    public string ExtraInfo { get => _extraInfo; set => _extraInfo = value; }
    
    public bool IsDepleted => _quantity <= 0 || _durability <= 0;

    public ItemInstance(ItemProfile itemProfile, int initialQuantity = 0, float initialDurability = 1, string extraInfo = "")
    {
        if (itemProfile == null)
        {
            throw new ArgumentNullException("아이템 정보는 null일 수 없습니다.");
        }
        
        ItemProfile = itemProfile;
        MaxQuantity = itemProfile.ItemDefinition.MaxQuantity;
        MaxDurability = itemProfile.ItemDefinition.MaxDurability;

        if (initialQuantity < 1 || initialQuantity > MaxQuantity)
        {
            throw new Exception("초기 수량은 1 이상 최대 수량 이하여야 합니다.");
        }

        if (initialDurability < 1 || initialDurability > MaxDurability)
        {
            throw new Exception("초기 내구도는 1 이상 최대 내구도 이하여야 합니다.");
        }
        
        _durability = initialDurability;
        _quantity = initialQuantity;
        _extraInfo = extraInfo;
    }

    public void Use(GameObject target, float amount = 1)
    {
        if (!ItemProfile.TryUseItem(target))
        {
            return;
        }

        if (ItemProfile.ItemDefinition.HasDurability)
        {
            TryReduceDurability(amount);
            Debug.Log(_durability);
        }
        else
        {
            TryRemove((int)amount);
            Debug.Log(_quantity);
        }
    }

    // 수량 제어 함수

    public void SetQuantity(int quantity)
    {
        if (quantity < 0 || quantity > MaxQuantity)
        {
            throw new Exception("수량은 0 이상 최대 수량 이하여야 합니다.");
        }

        _quantity = quantity;
    }

    public bool TryAdd(int amount)
    {
        if (amount < 0)
        {
            return false;
        }

        if (_quantity + amount > MaxQuantity)
        {
            return false;
        }

        _quantity += amount;
        return true;
    }

    public bool TryRemove(int amount)
    {
        if (amount < 0)
        {
            return false;
        }

        if (_quantity - amount < 0)
        {
            return false;
        }

        _quantity -= amount;
        return true;
    }

    // 내구도 제어 함수

    public void SetDurability(float durability)
    {
        if (durability < 0f || durability > MaxDurability)
        {
            throw new Exception("내구도는 0 이상 최대 내구도 이하여야 합니다.");
        }

        _durability = durability;
    }

    public bool TryAddDurability(float amount)
    {
        if (amount < 0f)
        {
            return false;
        }

        if (_durability + amount > MaxDurability)
        {
            return false;
        }

        _durability += amount;
        return true;
    }

    public bool TryReduceDurability(float amount)
    {
        if (amount < 0f)
        {
            return false;
        }

        if (_durability - amount < 0f)
        {
            return false;
        }

        _durability -= amount;
        return true;
    }
}