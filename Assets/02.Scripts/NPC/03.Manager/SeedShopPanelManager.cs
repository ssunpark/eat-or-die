using System;
using System.Linq;
using UnityEngine;

public class SeedShopPanelManager : BehaviourSingleton<SeedShopPanelManager>
{
    private ItemProfile _itemProfile;
    private ItemProfile[] _seedItems;
    public ItemProfile[] SeedItems => _seedItems;
    public event Action OnSeedListUpdated;
    
    [SerializeField] private int npcId = 1200002; // 모종 상인 NPC ID
    public UI_SeedItemDetail SeedItemDetailUI;
    public UI_NpcDialogue NpcDialogueUI;
    
    private void Start()
    {
        // NpcItemList가 이미 로드되어 있으면 바로 실행
        if (NpcDataManager.Instance != null && NpcDataManager.Instance.NpcItemList != null)
        {
            HandleNpcItemListLoaded();
        }
    }

    private void HandleNpcItemListLoaded()
    {
        LoadSeedItemsFromNpc(npcId);
        
        if (_seedItems != null && _seedItems.Length > 0)
        {
            _itemProfile = _seedItems[0];
            UpdateSeedDetail(_itemProfile.ItemDefinition.ID);
        }
        
        else
        {
            Debug.Log("[SeedShop] 시드 아이템이 비어 있어 초기화 실패");
        }
    }
    
    public void LoadSeedItemsFromNpc(int npcId)
    {
        if (NpcDataManager.Instance == null || NpcDataManager.Instance.NpcItemList == null)
        {
            Debug.Log("[SeedShop] NpcItemList가 아직 로드되지 않았습니다.");
            return;
        }
        _seedItems = NpcDataManager.Instance.NpcItemList
            .Where(item => item.NpcID == npcId && item.ItemID >= 100000 && item.ItemID < 200000)
            .Select(item => ItemManager.Instance.GetItem(item.ItemID))
            .Where(itemInfo => itemInfo != null)
            .ToArray();

        Debug.Log($"[SeedShop] 시드 아이템 개수: {_seedItems.Length}");

        for (int i = 0; i < _seedItems.Length; i++)
        {
            Debug.Log($"[SeedShop] SeedItem: {_seedItems[i].ItemDefinition.ID} - {_seedItems[i].ItemDefinition.Name}");
        }

        OnSeedListUpdated?.Invoke();
    }
    
    public void UpdateSeedDetail(int seedItemID)
    {
        ItemProfile selected = _seedItems.FirstOrDefault(x => x.ItemDefinition.ID == seedItemID);
        NpcItem npcItem = NpcDataManager.Instance.NpcItemList
            .FirstOrDefault(x => x.NpcID == npcId && x.ItemID == seedItemID);

        if (selected != null && npcItem != null)
        {
            SeedItemDetailUI.SetDetail(selected, npcItem);
        }
        
        UpdateNpcDialogue(npcId);
    }



    public void UpdateNpcDialogue(int npcID)
    {
        var dialogueList = NpcDataManager.Instance.NpcDialogueList
            .Where(d => d.NPCID == npcID)
            .ToList();

        if (dialogueList.Count == 0)
        {
            Debug.Log($"[NpcDialogue] NPC ID {npcID}의 대사가 없습니다.");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, dialogueList.Count);
        string randomDialogue = dialogueList[randomIndex].DialogueContents;

        NpcDialogueUI.Setup(randomDialogue);
    }
}