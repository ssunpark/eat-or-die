using UnityEngine;

public class InventoryTest : MonoBehaviour
{
	public void OnClickGenerateItemButton(int itemID)
	{
		InventoryManager.Instance.PickItemFromGround(new Item(ItemManager.Instance.GetItem(itemID), 99, 1));
	}
}
