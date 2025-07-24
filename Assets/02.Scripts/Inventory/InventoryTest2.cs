using UnityEngine;

public class InventoryTest2 : MonoBehaviour
{
	public void OnClickTestButton()
	{
		InventoryManager.Instance.PickItemFromGround(new Item(3, 99, 1));
	}
}
