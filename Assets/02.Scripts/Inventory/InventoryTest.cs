using UnityEngine;

public class InventoryTest : MonoBehaviour
{
	public void OnClickGenerateItemButton(int itemID)
	{
		UnifiedInventoryManager.Instance.AddItem(new ItemInstance(ItemManager.Instance.GetItem(itemID), 1, 1));
	}
}
