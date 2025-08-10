using UnityEngine;
using UnityEngine.EventSystems;

public class UI_DropZone : MonoBehaviour, IPointerDownHandler
{
	private void Start()
	{
		gameObject.SetActive(false);
		HandEntity.Instance.OnItemPickedUp += ChangeDropZoneStatus;
	}
	
	private void ChangeDropZoneStatus()
	{
		gameObject.SetActive(!HandEntity.Instance.IsHandEmpty);
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (!HandEntity.Instance.IsHandEmpty)
			{
				ItemInstance itemInstanceInHand = HandEntity.Instance.ItemInstance;
				ItemManager.Instance.RPC_CreateItemObject(
					itemInstanceInHand.ID,
					itemInstanceInHand.Quantity, 
                    itemInstanceInHand.Durability,
					HandEntity.Instance.transform.position, 
					HandEntity.Instance.transform.rotation);
				HandEntity.Instance.DropItem();
			}
		}
	}
}
