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
        // 플레이어 Cooking FSM 호출!
       
        // CookingManager.Instance.TryStartCookRPC();
        CookingManager.Instance.TryStartCook();
        PopupManager.Instance.CloseAll();
        
        // 요리 결과물 테스트를 위해 추가된 임시 코드입니다.
        //CookingPanelManager.Instance.OnCookingCompleted(true);
        // CookingPanelManager.Instance.ProcessCookingResult(); // 수현 테스트 코드
    }

}
