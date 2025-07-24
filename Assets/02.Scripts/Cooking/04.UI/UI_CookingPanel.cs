using UnityEngine;
using UnityEngine.EventSystems;

//수현
public class UI_CookingPanel : MonoBehaviour
{
    public GameObject CookingPanel;
    public GameObject RecipePanel;

    private bool isOpen = false;

    private void Start()
    {
        CookingPanel.SetActive(false);
        RecipePanel.SetActive(false);
    }

    public void OnClickRecipeButton()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            RecipePanel.GetComponent<UI_RecipePanel>().Open();
        }
        else
        {
            RecipePanel.GetComponent<UI_RecipePanel>().Close();
        }
    }

    public void OpenCookingPanel()
    {
        CookingPanel.SetActive(true);
        InputReader.playerControllerInputBlocked = true;
    }

    public void OnClickCookingButton()
    {
        // 플레이어 Cooking FSM 호출!
        CookingPanelManager.Instance.StartCook();
        // 요리 결과물 테스트를 위해 추가된 임시 코드입니다.
        //CookingPanelManager.Instance.OnCookingCompleted(true);
        // CookingPanelManager.Instance.ProcessCookingResult(); // 수현 테스트 코드
        CloseTab();
    }

    private void CloseTab()
    {

        InputReader.playerControllerInputBlocked = false;
        isOpen = false;
        CookingPanel.SetActive(false);
        RecipePanel.SetActive(false);
    }
}
