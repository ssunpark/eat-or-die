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
        RecipePanel.SetActive(false);
    }

    public void OnClickRecipeButton()
    {
        isOpen = !isOpen;
        RecipePanel.SetActive(isOpen);
    }

    public void OnClickCookingButton()
    {
        CloseTab();
        // 레시피 조합에 따른 결과 출력 명령 메서드 호출
        CookingPanelManager.Instance.TryGetRecipeResult();
    }

    private void CloseTab()
    {
        isOpen = false;
        CookingPanel.SetActive(false);
        RecipePanel.SetActive(false);
    }
}
