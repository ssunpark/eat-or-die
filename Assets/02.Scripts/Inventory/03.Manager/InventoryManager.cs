using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Analytics;

public class InventoryManager : BehaviourSingleton<InventoryManager>
{
    private Inventory _inventory;
    public int InventorySize;

    private InventoryRepository _repository;
    
    public event Action<int> OnSlotUpdated;
    public event Action<bool> OnToggleInventory;
    public event Action OnInventoryUpdated;
    
    public void Open() => ToggleInventory(true);
    public void Close() => ToggleInventory(false);

    private async void Awake()
    {
        _inventory = new Inventory(InventorySize);
        
        await FirebaseManager.Instance.WaitForInitialization();
        _repository = new InventoryRepository(FirebaseManager.Instance.DB);
        OnInventoryUpdated += UpdateInventoryRepository;
        Init();
    }

    private async void Init()
    {
        List<SlotDTO> loadedInventory = await _repository.LoadInventoryItemList(AuthenticationManager.Instance.User.UserId, CharacterInfoManager.Instance.CharacterInfo.Id);
        
        foreach (SlotDTO slot in loadedInventory)
        {
            if (slot.ItemId == 0)
            {
                continue;
            }
            ItemInstance item = new ItemInstance(ItemManager.Instance.GetItem(slot.ItemId), slot.Quantity, slot.Durability, slot.ExtraInfo);
            _inventory.PutItemInSlot(int.Parse(slot.SlotId), item);
        }
    }

    private async void UpdateSlotRepository(int slotIndex)
    {
        SlotDTO slot = new SlotDTO(slotIndex, GetItemInSlot(slotIndex));
        await _repository.SaveInventoryItem(AuthenticationManager.Instance.User.UserId, CharacterInfoManager.Instance.CharacterInfo.Id, slot);
    }

    private void UpdateInventoryRepository()
    {
        for (int i = 0; i < InventorySize; ++i)
        {
            UpdateSlotRepository(i);
        }
    }

    public void ToggleInventory(bool toggle)
    {
        OnToggleInventory?.Invoke(toggle);
    }
    // UIGlobalManager가 관리하게 바꿨습니다. 만약 인벤토리를 열때 무언가 초기화가 필요하다면...
    // 몰라

    public void OnClickMouseLeft(int slotIndex)
    {
        if (HandEntity.Instance.IsHandEmpty)
        {
            ItemInstance itemInstanceInSlot = _inventory.PopItemInSlot(slotIndex);
            if (itemInstanceInSlot == null) return;
            
            HandEntity.Instance.PickUpItem(itemInstanceInSlot);
        }
        else
        {
            HandEntity.Instance.PickUpItem(_inventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemInstance));
        }
        UpdateSlotRepository(slotIndex);
        OnSlotUpdated?.Invoke(slotIndex);
        OnInventoryUpdated?.Invoke();
    }
    
    public void OnClickMouseRight(int slotIndex)
    {
        if (_inventory.SlotList[slotIndex].IsEmpty) return;
        
        if (HandEntity.Instance.IsHandEmpty)
        {
            HandEntity.Instance.PickUpItem(_inventory.PopSingleItemInSlot(slotIndex));
        }
        else
        {
            if (HandEntity.Instance.ItemInstance.ID == _inventory.SlotList[slotIndex].ItemInstance.ID)
            {
                ItemInstance itemInstanceInSlot = _inventory.PopSingleItemInSlot(slotIndex);
                if (!HandEntity.Instance.TryAddItem(itemInstanceInSlot))
                {
                    _inventory.SlotList[slotIndex].ItemInstance.TryAdd(itemInstanceInSlot.Quantity);
                }
            }
            else
            {
                ItemInstance temp = _inventory.PopItemInSlot(slotIndex);
                _inventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemInstance);
                HandEntity.Instance.PickUpItem(temp);
            }
        }
        UpdateSlotRepository(slotIndex);
        OnSlotUpdated?.Invoke(slotIndex);
        OnInventoryUpdated?.Invoke();
    }

    public ItemInstance AddItemToInventory(ItemInstance itemInstance)
    {
        ItemInstance remain = _inventory.AddItemToInventory(itemInstance);
        
        OnInventoryUpdated?.Invoke();
        
        return remain;
    }

    public ItemInstance AddItemToEmptySlot(ItemInstance itemInstance)
    {
        ItemInstance remain = _inventory.AddItemToEmptySlot(itemInstance);
        
        OnInventoryUpdated?.Invoke();

        return remain;
    }

    public bool HaveItem(int itemID)
    {
        return _inventory.HaveItem(itemID);
    }

    public int GetItemCount(int itemID)
    {
        return _inventory.GetItemCount(itemID);
    }

    public int RequestConsumeItem(int itemID, int amount)
    {
        int consumed = GetItemCount(itemID);

        if (amount < consumed)
        {
            consumed = amount;
        }
        
        TryConsumeItem(itemID, consumed);
        
        return consumed;
    }
    
    public bool TryConsumeItem(int itemID, int amount)
    {
        bool result = _inventory.TryConsumeItem(itemID, amount);
        
        if (result)
        {
            OnInventoryUpdated?.Invoke();
        }
        return result;
    }

    public ItemInstance GetItemInSlot(int slotIndex)
    {
        return _inventory.GetItemInSlot(slotIndex);
    }
    
    public List<Slot> GetAllSlots()
    {
        return _inventory.GetAllSlots();
    }

    public void DropAllItems(Vector3 position)
    {
        List<Slot> slots = GetAllSlots();

        foreach (Slot slot in slots)
        {
            if (!slot.IsEmpty)
            {
                ItemInstance item = slot.GetItem();
                ItemProxySpawner.Instance.RPC_CreateItemObject(item.ID, item.Quantity, item.Durability, position, Quaternion.identity, item.ExtraInfo);
                slot.RemoveItem();
            }
        }
        OnInventoryUpdated?.Invoke();
    }
}
