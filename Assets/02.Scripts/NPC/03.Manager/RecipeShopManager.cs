using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RecipeShopManager : BehaviourSingleton<RecipeShopManager>
{
    private ItemProfile _itemProfile;
    private ItemProfile[] _recipeItems;
    public ItemProfile[] RecipeItems => _recipeItems;
    public event Action OnRecipeListUpdated;

    private int npcId = 1200003;
    public UI_RecipeItemDetail RecipeItemDetailUI;
    public UI_NpcDialogue NpcDialogueUI;

    private void Awake()
    {
        RecipeShopNpcInteractable.PanelOpened += OpenRecipeShopUI;
    }

    private void Start()
    {
        // 레시피 해금 이벤트 구독
        if (RoomRecipeStateManager.Instance != null)
        {
            RoomRecipeStateManager.Instance.OnRecipeUnlocked += OnRecipeUnlockedHandler;
        }
    }

    private void OnRecipeUnlockedHandler(Recipe unlockedRecipe)
    {
        Debug.Log($"[RecipeShop] 레시피 해금됨: {unlockedRecipe.ID} - 상점 목록 업데이트");
        UpdateRecipeShopList();
    }

    public void OpenRecipeShopUI()
    {
        Debug.Log("[RecipeShop] OpenRecipeShopUI 호출됨");
        UpdateRecipeShopList();

        if (_recipeItems != null && _recipeItems.Length > 0)
        {
            _itemProfile = _recipeItems[0];
            UpdateRecipeDetail(_itemProfile.ItemDefinition.ID);
            Debug.Log($"[RecipeShop] {_recipeItems.Length}개의 레시피 아이템 로드 완료");
        }
        else
        {
            Debug.Log("[RecipeShop] 레시피 아이템이 비어 있어 초기화 실패");
        }

        UpdateNpcDialogue(npcId);

        // UI 업데이트를 위해 이벤트 발생
        OnRecipeListUpdated?.Invoke();
    }

    public void UpdateRecipeShopList()
    {
        if (RecipeManager.Instance == null || RoomRecipeStateManager.Instance == null)
        {
            Debug.Log("[RecipeShop] Manager들이 아직 로드되지 않았습니다.");
            return;
        }

        // 1. 모든 음식 레시피 가져오기
        var allRecipes = RecipeManager.Instance.GetRecipesByCategory(ERecipeCategory.Food);
        
        // 2. 해금되지 않은 레시피만 필터링
        var unlockedRecipes = allRecipes.Where(recipe => 
            !RoomRecipeStateManager.Instance.IsUnlockedRecipes(recipe.ID))
            .ToList();
        
        // 3. 최대 8개로 제한
        var selectedRecipes = unlockedRecipes.Take(8).ToList();
        
        // 4. ItemProfile 배열로 변환
        _recipeItems = selectedRecipes
            .Select(recipe => ItemManager.Instance.GetItem(recipe.ResultID))
            .Where(item => item != null)
            .ToArray();

        OnRecipeListUpdated?.Invoke();
    }

    public void UpdateRecipeDetail(int recipeITemID)
    {
        ItemProfile selected = _recipeItems.FirstOrDefault(x => x.ItemDefinition.ID == recipeITemID);
        NpcItem npcItem =
            NpcDataManager.Instance.NpcItemList.FirstOrDefault(x => x.NpcID == npcId && x.ItemID == recipeITemID);
        
        if (selected != null && npcItem != null)
        {
            Debug.Log($"[RecipeShop] 선택된 레시피 아이템: {selected.ItemDefinition.ID} - {selected.ItemDefinition.Name}");
            RecipeItemDetailUI.SetDetail(selected, npcItem);
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
            return;
        }

        int randomIndex = Random.Range(0, dialogueList.Count);
        string randomDialogue = dialogueList[randomIndex].DialogueContents;

        NpcDialogueUI.Setup(randomDialogue);
    }
}
