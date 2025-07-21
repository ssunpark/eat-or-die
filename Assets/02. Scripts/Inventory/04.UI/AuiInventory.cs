using System.Collections.Generic;
using UnityEngine;

public class AuiInventory : AUI_PopupBase
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

        InventoryManager.Instance.OnInventoryUpdated += UpdateInventoryUI;
        gameObject.SetActive(false);
    }

    public void ToggleInventory()
    {
        bool isActive = gameObject.activeSelf;
        gameObject.SetActive(!isActive);
    }
    
    private void UpdateInventoryUI()
    {
        foreach (UI_InventorySlot uiSlot in _uiSlotList)
        {
            uiSlot.UpdateSlotUI();
        }
    }

}
