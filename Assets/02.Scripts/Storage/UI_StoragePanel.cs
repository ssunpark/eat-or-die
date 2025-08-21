using System.Collections.Generic;
using UnityEditor.Rendering;
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

        SharedStorageManager.Instance.OnStorageUpdated += UpdateInventoryUI;
        SharedStorageManager.Instance.OnOpenStorage += Open;
        gameObject.SetActive(false);
    }

    public override void Open()
    {
        InventoryManager.Instance.Open();
        base.Open();
    }
    
    public override void Close()
    {
        base.Close();
        InventoryManager.Instance.Close();
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