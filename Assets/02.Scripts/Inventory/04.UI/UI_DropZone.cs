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
				Item itemInHand = HandEntity.Instance.Item;
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
