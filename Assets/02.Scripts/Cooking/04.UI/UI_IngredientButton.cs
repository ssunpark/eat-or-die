using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 수현
public class UI_IngredientButton : MonoBehaviour
{
    public TextMeshProUGUI IngredientNameTextUI;
    public Button  IngredientButtonUI;
    private IngredientCSVData _data;
    
    public int IngredientID { get; private set; }

    private void Start()
    {
        IngredientButtonUI.onClick.AddListener(OnClickButton);
    }
    
    public void Refresh(IngredientCSVData data)
    {
        _data = data;
        IngredientNameTextUI.text = _data.Name;
        IngredientID = _data.ID;
    }

    public void OnClickButton()
    {
        RecipePanelManager.Instance.UpdateRecipes(IngredientID);
    }
}
