using UnityEngine;
using UnityEngine.EventSystems;

public class UI_DropZone : MonoBehaviour, IPointerDownHandler
{
	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (!HandEntity.Instance.IsHandEmpty)
			{
				ItemStack itemInHand = HandEntity.Instance.ItemStack;
				ItemManager.Instance.RPC_CreateItemObject(
					itemInHand.ID,
					itemInHand.Quantity, 
					HandEntity.Instance.transform.position, 
					HandEntity.Instance.transform.rotation);
				HandEntity.Instance.DropItem();
			}
		}
	}
}
