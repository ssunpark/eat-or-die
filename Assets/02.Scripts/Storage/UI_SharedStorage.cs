using System.Collections.Generic;
using UnityEngine;

public class UI_SharedStorage : AUI_PopupBase
{
	public override EPopupType Type => EPopupType.Inventory;
	//
	// [SerializeField] private GameObject _uiSlotPrefab;
	// private List<UI_InventorySlot> _uiSlotList = new List<UI_InventorySlot>();
	//
	// private void Start()
	// {
	// 	SharedStorageManager.Instance.OnStorageUpdated += UpdateInventoryUI;
	// 	gameObject.SetActive(false);
	// }
	//
	// public void ToggleInventory()
	// {
	// 	bool isActive = gameObject.activeSelf;
	// 	gameObject.SetActive(!isActive);
	// }
	//
	// private void UpdateInventoryUI()
	// {
	// 	foreach (UI_InventorySlot uiSlot in _uiSlotList)
	// 	{
	// 		uiSlot.UpdateSlotUI();
	// 	}
	// }
	//
	// private void UpdateSlotUI(int slotIndex)
	// {
	// 	_uiSlotList[slotIndex].UpdateSlotUI();
	// }	
}
