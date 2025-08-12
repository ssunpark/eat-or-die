using System;
using UnityEngine;

public class SharedStorageManager : BehaviourSingleton<SharedStorageManager>
{
    [SerializeField] private SharedStorage _currentSharedStorage;

    public event Action OnStorageUpdated;
    public event Action OnOpenStorage;

    public void RegisterStorage(SharedStorage sharedStorage)
    {
        if (_currentSharedStorage != null)
        {
            _currentSharedStorage.OnStorageUpdated -= OnStorageUpdated;
        }
        
        _currentSharedStorage = sharedStorage;
        _currentSharedStorage.OnStorageUpdated += OnStorageUpdated;
        OnStorageUpdated?.Invoke(); // UI 업데이트
        OnOpenStorage?.Invoke(); // 창고 열기 이벤트
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

    public void OnClickMouseLeft(int slotIndex)
    {
        if (HandEntity.Instance.IsHandEmpty)
        {
            _currentSharedStorage.RPC_TryTakeItem(slotIndex);
        }
        else
        {
            NetworkedItem item = HandEntity.Instance.GetItem().ToNetworkedItem();
            _currentSharedStorage.RPC_TryPutItem(slotIndex, item);
        }
    }

    public void OnClickMouseRight(int slotIndex)
    {
        if (HandEntity.Instance.IsHandEmpty)
        {
            _currentSharedStorage.RPC_TryTakeOneItem(slotIndex, new NetworkedItem { ID = 0 });
        }
        else
        {
            NetworkedItem item = HandEntity.Instance.GetItem().ToNetworkedItem();
            _currentSharedStorage.RPC_TryTakeOneItem(slotIndex, item);
        }
    }
    
    public NetworkedItem GetItemInSlot(int slotIndex)
    {
        return _currentSharedStorage.GetItemInSlot(slotIndex);
    }
}