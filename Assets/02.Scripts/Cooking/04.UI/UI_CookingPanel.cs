using DarkTonic.MasterAudio;
using Fusion;
using TMPro;
using UnityEngine;

public class UI_CookingPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Cook;
    public GameObject CookingPanel;
    public UI_RecipeIngredient UIRecipeIngredient;
    public UI_RecipeList UIRecipeList;
    public TextMeshProUGUI IngredientNameText;
    private bool _isInitialized;

    private void Start()
    {
        CookingPanel.SetActive(false);
    }

    public override void Open()
    {
        MasterAudio.PlaySound("CookingPotInteract");
        InventoryManager.Instance.ToggleInventory(true);
        base.Open();
        if (!_isInitialized)
        {
            Init();
            _isInitialized = true;
        }
    }

    private void Init()
    {
        // RecipePanelUIManager에 CookingPanel 참조 설정
        RecipePanelUIManager.Instance.SetCookingPanel(this);
        
        UIRecipeIngredient.PopulateIngredients(ERecipeCategory.Food);
        UIRecipeList.Init();
        
        // 모든 UI 초기화 완료 후 Food 카테고리 레시피 표시 및 텍스트 설정
        RecipePanelUIManager.Instance.UpdateAllRecipes();
    }

    public void OnClickCookingButton()
    {
        NetworkRunner Runner = FindAnyObjectByType<NetworkRunner>();
        if (Runner == null)
        {
            Debug.Log("NetworkRunner를 찾을 수 없습니다!");
            return;
        }

        CookingManager.Instance.TryStartCook();
        PopupManager.Instance.CloseAll();
    }

    public override void Close()
    {
        base.Close();
        InventoryManager.Instance.ToggleInventory(false);
        MasterAudio.StopAllOfSound("CookingPotInteract");
    }
}
