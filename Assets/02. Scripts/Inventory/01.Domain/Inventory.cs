using System.Collections.Generic;

public class Inventory
{
    private int _inventorySize;
    
    public List<Slot> Slots { get; private set; }

    public Inventory(int inventorySize)
    {
        _inventorySize = inventorySize;
        Slots = new List<Slot>(_inventorySize);
    }
}
