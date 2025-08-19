using Fusion;
using UnityEngine;

//수현
public class UI_CookingPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Cook;
    public GameObject CookingPanel;
    public UI_RecipeIngredient UIRecipeIngredient;
    public UI_RecipeList UIRecipeList;
    private bool _isInitialized;

    private void Start()
    {
        CookingPanel.SetActive(false);
    }

    public override void Open()
    {
        base.Open();
        if (!_isInitialized)
        {
            Init();
            _isInitialized = true;
        }
    }

    private void Init()
    {
        UIRecipeIngredient.Init();
        UIRecipeList.Init();
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

}
