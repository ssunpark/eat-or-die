using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

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
                ItemProxySpawner.Instance.RPC_CreateItemObject(
					itemInstanceInHand.ID,
					itemInstanceInHand.Quantity, 
                    itemInstanceInHand.Durability,
					Room.Instance.LocalPlayer.transform.position, 
					Room.Instance.LocalPlayer.transform.rotation);
				HandEntity.Instance.DropItem();
			}
		}
		else if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (!HandEntity.Instance.IsHandEmpty)
			{
				UnifiedInventoryManager.Instance.AddItem(HandEntity.Instance.ItemInstance);
				HandEntity.Instance.DropItem();
			}
		}
	}
}
