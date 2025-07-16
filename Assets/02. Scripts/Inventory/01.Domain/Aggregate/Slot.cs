public class Slot
{
    public ItemStack ItemStack { get; private set; }
    public bool IsEmpty => ItemStack == null;
    
    public void AddItem(ItemStack itemStack)
    {
        ItemStack = itemStack;
    }

    public void RemoveItem()
    {
        ItemStack = null;
    }
}
