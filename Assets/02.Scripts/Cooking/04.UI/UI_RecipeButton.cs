using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 수현
public class UI_RecipeButton : MonoBehaviour
{
    // public TextMeshProUGUI RecipeNameTextUI;
    public Image IconImage;
    private RecipeCSVData _data;
    public void Refresh(RecipeCSVData Data)
    {
        _data = Data;
        // RecipeNameTextUI.text = Data.Name;
        
        AItem item = ItemManager.Instance.GetItem(_data.ResultID);
        if (item != null)
        {
            IconImage.sprite = item.ItemData.Icon;
            IconImage.gameObject.SetActive(true);
        }
        else
        {
            {
                Debug.LogWarning($"[UI_IngredientButton] 아이템 데이터 없음 - ID: {_data.ID}");
                IconImage.gameObject.SetActive(false);
            }
        }
    }
}
