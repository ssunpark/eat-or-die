using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _uiSlotPrefab;
    private List<UI_Slot> _uiSlotList = new List<UI_Slot>();

    private void Start()
    {
        for (int i = 0; i < InventoryManager.Instance.InventorySize; ++i)
        {
            GameObject slot = Instantiate(_uiSlotPrefab, transform);
            UI_Slot slotComponent = slot.GetComponent<UI_Slot>();
            slot.transform.SetSiblingIndex(i);
            slotComponent.Initialize(i);
            _uiSlotList.Add(slotComponent);
        }
        gameObject.SetActive(false);
    }

    public void ToggleInventory()
    {
        bool isActive = gameObject.activeSelf;
        gameObject.SetActive(!isActive);
    }
    
}
