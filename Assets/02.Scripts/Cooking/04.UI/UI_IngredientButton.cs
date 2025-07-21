using TMPro;
using UnityEngine;
// 수현
public class UI_IngredientButton : MonoBehaviour
{
    public TextMeshProUGUI IngredientNameTextUI; 
        
    private IngredientCSVData _data;

    public void Refresh(IngredientCSVData data)
    {
        _data = data;
        IngredientNameTextUI.text = _data.Name;
    }
}
