using System;

// 외부에서 보이는 값이 변하는 아이템 실질적인 객체
// 내구도, 갯수 유형으로 나뉨
public class Item
{
    public readonly int ID;
    public readonly int MaxQuantity;
    
    private int _quantity;
    public int Quantity => _quantity;

    public Item(int id, int maxQuantity, int initialQuantity = 0)
    {
        if (id < 0)
        {
            throw new Exception("아이템 ID은 음수가 아닙니다.");
        }

        if (maxQuantity < 1)
        {
            throw new Exception("아이템 최대 갯수는 1 이상입니다.");
        }

        if (initialQuantity < 0 || initialQuantity > maxQuantity)
        {
            throw new Exception("수량은 0 이상 최대 수량 이하여야 합니다.");
        }

        ID = id;
        MaxQuantity = maxQuantity;
        _quantity = initialQuantity;
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
}