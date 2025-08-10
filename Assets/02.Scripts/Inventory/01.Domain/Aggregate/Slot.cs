public class Slot
{
    public ItemInstance ItemInstance { get; private set; }
    public bool IsEmpty => ItemInstance == null;
    
    public void AddItem(ItemInstance itemInstance)
    {
        ItemInstance = itemInstance;
    }

    public void RemoveItem()
    {
        ItemInstance = null;
    }

    public void UseItem()
    {
        ItemInstance.TryRemove(1);
        if (ItemInstance.Quantity == 0)
        {
            RemoveItem();
        }
    }
    
}
