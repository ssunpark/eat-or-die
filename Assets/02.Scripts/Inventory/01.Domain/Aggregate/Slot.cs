public class Slot
{
    public Item Item { get; private set; }
    public bool IsEmpty => Item == null;
    
    public void AddItem(Item item)
    {
        Item = item;
    }

    public void RemoveItem()
    {
        Item = null;
    }

    public void UseItem()
    {
        Item.TryRemove(1);
        if (Item.Quantity == 0)
        {
            RemoveItem();
        }
    }
    
}
