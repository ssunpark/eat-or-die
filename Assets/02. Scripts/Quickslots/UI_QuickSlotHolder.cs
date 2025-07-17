using System.Collections.Generic;
using UnityEngine;

public class UI_QuickSlotHolder : MonoBehaviour
{
	[SerializeField] private GameObject _quickSlotPrefab;
	private List<UI_QuickSlot> _quickSlotList = new List<UI_QuickSlot>();

	private void Start()
	{
		for (int i = 0; i < QuickSlotManager.Instance.QuickSlotSize; ++i)
		{
			GameObject slot = Instantiate(_quickSlotPrefab, transform);
			UI_QuickSlot quickSlotComponent = slot.GetComponent<UI_QuickSlot>();
			slot.transform.SetSiblingIndex(i);
			quickSlotComponent.Initialize(i);
			_quickSlotList.Add(quickSlotComponent);
		}

		// QuickSlotManager.Instance.OnQuickSlotUpdated += UpdateQuickSlots;
	}

	// private void UpdateQuickSlots()
	// {
	// 	foreach (UI_QuickSlot quickSlot in _quickSlotList)
	// 	{
	// 		
	// 	}
	// }
}
