using UnityEngine;

public class InventoryTest : MonoBehaviour
{
	public void OnClickGenerateItemButton(int itemID)
	{
		InventoryManager.Instance.PickItemFromGround(new ItemInstance(ItemManager.Instance.GetItem(itemID), 1, 1));
	}
}
