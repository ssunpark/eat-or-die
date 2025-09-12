using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SeedShopManager : BehaviourSingleton<SeedShopManager>
{
    private ItemProfile _itemProfile;
    private ItemProfile[] _seedItems;
    public ItemProfile[] SeedItems => _seedItems;
    public event Action OnSeedListUpdated;

    [SerializeField] private int npcId = 1200002;
    public UI_SeedItemDetail SeedItemDetailUI;
    public UI_NpcDialogue NpcDialogueUI;

    private void Awake()
    {
        SeedShopNpcInteractable.PanelOpened += OpenSeedShopUI;
    }

    public void OpenSeedShopUI()
    {
        UpdateSeedShopList(npcId);
        
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
    
    public void UpdateSeedShopList(int npcId)
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

        OnSeedListUpdated?.Invoke();
    }
    
    public void UpdateSeedDetail(int seedItemID)
    {
        ItemProfile selected = _seedItems.FirstOrDefault(x => x.ItemDefinition.ID == seedItemID);
        NpcItem npcItem = NpcDataManager.Instance.NpcItemList
            .FirstOrDefault(x => x.NpcID == npcId && x.ItemID == seedItemID);

        if (selected != null && npcItem != null)
        {
            Debug.Log($"[SeedShop] 선택된 시드 아이템: {selected.ItemDefinition.ID} - {selected.ItemDefinition.Name}");
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

        int randomIndex = Random.Range(0, dialogueList.Count);
        string randomDialogue = dialogueList[randomIndex].DialogueContents;

        NpcDialogueUI.Setup(randomDialogue);
    }
}