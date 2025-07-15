using System.Collections.Generic;

public class Inventory
{
    private int _inventorySize;
    
    public List<Slot> SlotList { get; private set; }

    public Inventory(int inventorySize)
    {
        _inventorySize = inventorySize;
        SlotList = new List<Slot>(_inventorySize);
    }
}
