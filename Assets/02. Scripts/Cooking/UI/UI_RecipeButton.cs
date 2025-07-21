using TMPro;
using UnityEngine;
// 뿡
public class UI_RecipeButton : MonoBehaviour
{
    public TextMeshProUGUI RecipeNameTextUI;
    private RecipeCSVData _data;
    public void Refresh(RecipeCSVData Data)
    {
        _data = Data;
        RecipeNameTextUI.text = Data.Name;
    }
}
