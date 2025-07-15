using UnityEngine;
//수현
public class UI_Cooking : MonoBehaviour
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
        isOpen = false;
        Debug.Log($"{isOpen}");
        CookingPanel.SetActive(false);
        RecipePanel.SetActive(false);
    }
}
