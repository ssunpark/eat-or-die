using System.Collections.Generic;
using UnityEngine;

public class UI_StoragePanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Storage;

    [SerializeField] private GameObject _uiSlotPrefab;
    private List<UI_StorageSlot> _uiSlotList = new List<UI_StorageSlot>();

    private void Start()
    {
        for (int i = 0; i < InventoryManager.Instance.InventorySize; ++i)
        {
            GameObject slot = Instantiate(_uiSlotPrefab, transform);
            UI_StorageSlot slotComponent = slot.GetComponent<UI_StorageSlot>();
            slot.transform.SetSiblingIndex(i);
            slotComponent.Initialize(i);
            _uiSlotList.Add(slotComponent);
        }

        SharedStorageManager.Instance.OnSlotUpdated += UpdateSlotUI;
        SharedStorageManager.Instance.OnStorageUpdated += UpdateInventoryUI;
        SharedStorageManager.Instance.OnOpenStorage += ToggleStorage;
        Close();
    }
    
    public void ToggleStorage()
    {
        bool isActive = gameObject.activeSelf;

        if (isActive)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void UpdateInventoryUI()
    {
        foreach (UI_StorageSlot uiSlot in _uiSlotList)
        {
            uiSlot.UpdateSlotUI();
        }
    }

    private void UpdateSlotUI(int slotIndex)
    {
        _uiSlotList[slotIndex].UpdateSlotUI();
    } 
}