using System;

// 외부에서 보이는 값이 변하는 아이템 실질적인 객체
// 내구도, 갯수 유형으로 나뉨
public class Item
{
    public readonly AItemInfo ItemInfo;
    public readonly int MaxQuantity;
    public readonly float MaxDurability;

    public int ID => ItemInfo.ItemData.ID;

    private int _quantity;
    public int Quantity => _quantity;

    private float _durability;
    public float Durability => _durability;

    // 추가적인 아이템 정보
    private string _extraInfo;
    public string ExtraInfo { get => _extraInfo; set => _extraInfo = value; }

    public Item(AItemInfo itemInfo, int initialQuantity = 0, float initialDurability = 1, string extraInfo = "")
    {
        if (itemInfo == null)
        {
            throw new ArgumentNullException("아이템 정보는 null일 수 없습니다.");
        }
        
        ItemInfo = itemInfo;
        MaxQuantity = itemInfo.ItemData.MaxQuantity;
        MaxDurability = itemInfo.ItemData.MaxDurability;

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