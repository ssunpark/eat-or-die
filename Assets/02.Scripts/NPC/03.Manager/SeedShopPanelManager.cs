using System;
using System.Linq;
using UnityEngine;

public class SeedShopPanelManager : BehaviourSingleton<SeedShopPanelManager>
{
    private AItemInfo[] _seedItems;
    public AItemInfo[] SeedItems => _seedItems;
    public event Action OnSeedListUpdated;
    
    [SerializeField] private int npcId = 1200002; // 모종 상인 NPC ID
    public UI_SeedItemDetail SeedItemDetailUI;
    public UI_NpcDialogue NpcDialogueUI;
    
    private void Start()
    {
        // NpcItemList가 이미 로드되어 있으면 바로 실행
        if (NpcDataManager.Instance != null && NpcDataManager.Instance.NpcItemList != null)
        {
            LoadSeedItemsFromNpc(npcId);
            UpdateNpcDialogue(npcId);
        }
        else
        {
            // 아니라면 로딩 완료 이벤트 구독
            NpcDataManager.Instance.OnNpcItemListLoaded += HandleNpcItemListLoaded;
        }
    }

    private void HandleNpcItemListLoaded()
    {
        // 한 번만 실행되도록 구독 해제
        NpcDataManager.Instance.OnNpcItemListLoaded -= HandleNpcItemListLoaded;
        LoadSeedItemsFromNpc(npcId);
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
            Debug.Log($"[SeedShop] SeedItem: {_seedItems[i].ItemData.ID} - {_seedItems[i].ItemData.Name}");
        }

        OnSeedListUpdated?.Invoke();
    }
    
    public void UpdateSeedDetail(int seedItemID)
    {
        AItemInfo selected = _seedItems.FirstOrDefault(x => x.ItemData.ID == seedItemID);
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
            Debug.LogWarning($"[NpcDialogue] NPC ID {npcID}의 대사가 없습니다.");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, dialogueList.Count);
        string randomDialogue = dialogueList[randomIndex].DialogueContents;

        NpcDialogueUI.Setup(randomDialogue);
    }
}