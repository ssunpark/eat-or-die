using System;
using UnityEngine;
public class CookingPanelManager : BehaviourSingleton<CookingPanelManager>
{
    // 드래그앤드랍, 클릭
    private ItemStack[] _inputSlots = new ItemStack[2];

    public event Action OnCookingSlotUpdated;
    
    // 슬롯에 들어 있는 아이템 조회
    public ItemStack GetItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inputSlots.Length) return null;
        return _inputSlots[slotIndex];
    }

    // 슬롯에서 아이템 제거
    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inputSlots.Length) return;
        _inputSlots[slotIndex] = null;
        OnCookingSlotUpdated?.Invoke();
    }

    // 슬롯 비었는지 확인
    public bool CanPlaceItem(int slotIndex, ItemStack itemStack)
    {
        if (slotIndex < 0 || slotIndex >= _inputSlots.Length) return false;
        return _inputSlots[slotIndex] == null;
    }

    // 슬롯에 아이템 넣고 이벤트 호출
    public void PlaceItem(int slotIndex, ItemStack itemStack)
    {
        if (slotIndex < 0 || slotIndex >= _inputSlots.Length) return;
        _inputSlots[slotIndex] = itemStack;
        OnCookingSlotUpdated?.Invoke();
    }

    // 요리 조합 확인용
    public ItemStack[] GetAllInputItems()
    {
        return _inputSlots;
    }

    public bool TryGetRecipeResult()
    {
        return false;
    }
}
