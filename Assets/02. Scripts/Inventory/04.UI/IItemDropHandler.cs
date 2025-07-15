using UnityEngine;
using UnityEngine.EventSystems;

public interface IItemDropHandler
{
    public void SwapItems(IItemDropHandler from, IItemDropHandler to);
    public ItemStack GetItemStack();
    public bool CanPutItem(ItemStack itemStack);
}
