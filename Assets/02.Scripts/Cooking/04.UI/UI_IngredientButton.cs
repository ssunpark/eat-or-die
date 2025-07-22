using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 수현
public class UI_IngredientButton : MonoBehaviour
{
    // public TextMeshProUGUI IngredientNameTextUI;
    public Image IconImage;
    public Button  IngredientButtonUI;
    
    private IngredientCSVData _data;
    public int IngredientID { get; private set; }

    private void Start()
    {
        IngredientButtonUI.onClick.AddListener(OnClickButton);
        // IconImage.gameObject.SetActive(false);
    }
    
    public void Refresh(IngredientCSVData data)
    {
        _data = data;
        // IngredientNameTextUI.text = _data.Name;
        IngredientID = _data.ID;
        
        AItem item = ItemManager.Instance.GetItem(_data.ID);
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

    public void OnClickButton()
    {
        RecipePanelManager.Instance.UpdateRecipes(IngredientID);
    }
}
