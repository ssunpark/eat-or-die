using System;

// 갯수를 포함하는 아이템 정보
public class ItemStack
{
    public readonly string ID;
    public readonly EItemType Type;
    private readonly int _maxQuantity;
    private int _quantity;
    public int Quantity => _quantity;

    public ItemStack(string id, EItemType type, int maxQuantity, int initialQuantity = 0)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new Exception("아이템 ID 값이 존재하지 않습니다.");
        }

        if (maxQuantity < 1)
        {
            throw new Exception("아이템 최대 갯수는 1 이상입니다.");
        }

        if (initialQuantity < 0 || initialQuantity > _maxQuantity)
        {
            throw new Exception("수량은 0 이상 최대 수량 이하여야 합니다.");
        }

        ID = id;
        Type = type;
        _maxQuantity = maxQuantity;
        _quantity = initialQuantity;
    }

    // 수량 제어 함수

    public void SetQuantity(int quantity)
    {
        if (quantity < 0 || quantity > _maxQuantity)
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

        if (_quantity + amount > _maxQuantity)
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
}