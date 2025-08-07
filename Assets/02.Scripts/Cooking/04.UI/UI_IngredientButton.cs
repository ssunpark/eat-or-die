using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 수현
public class UI_IngredientButton : MonoBehaviour
{
    // public TextMeshProUGUI IngredientNameTextUI;
    public Image IconImage;
    public Button  IngredientButtonUI;
    public int IngredientID { get; private set; }

    private void Start()
    {
        IngredientButtonUI.onClick.AddListener(OnClickButton);
        // IconImage.gameObject.SetActive(false);
    }
    
    public void Refresh(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.Log("[UI_IngredientButton] 전달된 아이템 정보가 null입니다.");
            IconImage.gameObject.SetActive(false);
            return;
        }

        IngredientID = itemData.ID;

        if (itemData.Icon != null)
        {
            IconImage.sprite = itemData.Icon;
            IconImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log($"[UI_IngredientButton] 아이콘이 비어 있음 - ID: {IngredientID}");
            IconImage.gameObject.SetActive(false);
        }
    }

    public void OnClickButton()
    {
        RecipePanelUIManager.Instance.UpdateRecipes(IngredientID);
    }
}
