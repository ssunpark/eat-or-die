
public static class CheatConsole
{
	public static void GenerateItemButton(int itemID, int quantity = 1, float durability = 1)
	{
		UnifiedInventoryManager.Instance.AddItem(new ItemInstance(ItemManager.Instance.GetItem(itemID), quantity, durability));
	}
}
