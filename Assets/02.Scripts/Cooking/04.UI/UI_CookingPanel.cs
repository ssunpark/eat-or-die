using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

//수현
public class UI_CookingPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Cook;
    public GameObject CookingPanel;
    public GameObject RecipePanel;

    private void Start()
    {
        CookingPanel.SetActive(false);
        RecipePanel.SetActive(false);
    }

    public void OnClickRecipeButton()
    {
        if (PopupManager.Instance.IsOpen(EPopupType.Recipe))
        {
            PopupManager.Instance.GetOpenPopup(EPopupType.Recipe)?.Close();
        }
        else
        {
            RecipePanel.GetComponent<UI_RecipePanel>().Open();
        }
    }

    public void OpenCookingPanel()
    {
        bool isActive = CookingPanel.activeSelf;
        CookingPanel.SetActive(!isActive);
    }

    public void OnClickCookingButton()
    {
        NetworkRunner Runner = FindObjectOfType<NetworkRunner>();
        if (Runner == null)
        {
            Debug.Log("NetworkRunner를 찾을 수 없습니다!");
            return;
        }
        // 플레이어 Cooking FSM 호출!
       
        CookingManager.Instance.TryStartCookRPC();
        PopupManager.Instance.CloseAll();
        
        // 요리 결과물 테스트를 위해 추가된 임시 코드입니다.
        //CookingPanelManager.Instance.OnCookingCompleted(true);
        // CookingPanelManager.Instance.ProcessCookingResult(); // 수현 테스트 코드
    }

}
