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
    }

    public void OnClickCookingButton()
    {
        // 플레이어 Cooking FSM 호출!
        
        CloseTab();
    }

    private void CloseTab()
    {
        isOpen = false;
        CookingPanel.SetActive(false);
        RecipePanel.SetActive(false);
    }
}
