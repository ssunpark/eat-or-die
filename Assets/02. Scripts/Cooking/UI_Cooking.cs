using UnityEngine;
//수현
public class UI_Cooking : MonoBehaviour
{
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
}
