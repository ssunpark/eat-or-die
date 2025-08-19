using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Inventory;

    [SerializeField] private GameObject _uiSlotPrefab;
    private List<UI_InventorySlot> _uiSlotList = new List<UI_InventorySlot>();

    private void Start()
    {
        for (int i = 0; i < InventoryManager.Instance.InventorySize; ++i)
        {
            GameObject slot = Instantiate(_uiSlotPrefab, transform);
            UI_InventorySlot inventorySlotComponent = slot.GetComponent<UI_InventorySlot>();
            slot.transform.SetSiblingIndex(i);
            inventorySlotComponent.Initialize(i);
            _uiSlotList.Add(inventorySlotComponent);
        }

        InventoryManager.Instance.OnSlotUpdated += UpdateSlotUI;
        InventoryManager.Instance.OnInventoryUpdated += UpdateInventoryUI;
        InventoryManager.Instance.OnToggleInventory += ToggleInventory;
        Close();
    }

    public void ToggleInventory(bool toggle)
    {
        if (toggle)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    private void UpdateInventoryUI()
    {
        foreach (UI_InventorySlot uiSlot in _uiSlotList)
        {
            uiSlot.UpdateSlotUI();
        }
    }

    private void UpdateSlotUI(int slotIndex)
    {
        _uiSlotList[slotIndex].UpdateSlotUI();
    }

}
