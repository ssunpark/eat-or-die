using UnityEngine;

public class InventoryTest : MonoBehaviour
{
	public void OnClickTestButton()
	{
		InventoryManager.Instance.PickItemFromGround(new ItemStack(0, 99, 1));
	}
}
