using UnityEngine;

public class InventoryManager : BehaviourSingleton<InventoryManager>
{
    private Inventory _inventory;
    public int InventorySize;

    private void Awake()
    {
        _inventory = new Inventory(InventorySize);
    }

    public void OnClickMouseLeft(int slotIndex)
    {
    }
}
