using UnityEngine;

public class InventoryTest2 : MonoBehaviour
{
	public void OnClickTestButton()
	{
		InventoryManager.Instance.PickItemFromGround(new ItemStack(3, 99, 1));
	}
}
