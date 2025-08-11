using System;
using UnityEngine;

public class SharedStorageManager : BehaviourSingleton<SharedStorageManager>
{
    [SerializeField] private SharedStorage _currentSharedStorage;

    public event Action OnStorageUpdated;
    public event Action<int> OnSlotUpdated;

    public void OpenStorage(SharedStorage sharedStorage)
    {
        _currentSharedStorage = sharedStorage;
        OnStorageUpdated?.Invoke(); // UI 업데이트 구독
    }
    
    public void GetItemFromStorage(ItemInstance itemInstance)
    {
        // 일단은 손으로 보내는 것만 있긴한데 분기가 생긴다면 여기서 처리될듯?
        ItemToHand(itemInstance);
    }

    private void ItemToHand(ItemInstance itemInstance)
    {
        HandEntity.Instance.PickUpItem(itemInstance);
    }
}