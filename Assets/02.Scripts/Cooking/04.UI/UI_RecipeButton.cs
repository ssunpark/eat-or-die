using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RecipeButton : MonoBehaviour
{
    // public TextMeshProUGUI RecipeNameTextUI;
    public Image IconImage;
    public Button RecipeButton;
    public Color UnlockedColor = Color.white;
    public Color lockedColor = Color.gray;
    public Sprite unknownIcon; 
    
    private Recipe _data;
    public int RecipeID => _data.ID;
    public int ResultItemID => _data.ResultID;
    
    public void Refresh(Recipe Data)
    {
        _data = Data;
        // RecipeNameTextUI.text = Data.Name;
        
        bool isKnown = RecipePanelUIManager.Instance.IsKnownRecipe(_data.ID);
        bool canMake = RecipePanelUIManager.Instance.CanMakeRecipe(_data);
        
        AItemInfo itemInfo = ItemManager.Instance.GetItem(_data.ResultID);

        if (!isKnown) // 방 기준으로 습득되지 않은 레시피에 대해서
        {
            IconImage.sprite = unknownIcon;
            IconImage.color = lockedColor;
            IconImage.gameObject.SetActive(true);
            LockButton();
            return;
        }
        
        if (itemInfo != null)
        {
            IconImage.sprite = itemInfo.ItemData.Icon;
            IconImage.gameObject.SetActive(true);

            if (canMake) // 만들 수 있으면
            {
                UnlockButton();
            }
            else // 만들 수 없으면
            {
                LockButton();
            }
        }
        
        // else
        // {
        //     Debug.Log($"[UI_IngredientButton] 아이템 데이터 없음 - ID: {_data.ID}");
        //     IconImage.gameObject.SetActive(false);
        //     LockButton();
        // }
    }

    public void UnlockButton()
    {
        RecipeButton.interactable = true;
        IconImage.color = UnlockedColor;
    }
    
    public void LockButton()
    {
        RecipeButton.interactable = false;
        IconImage.color = lockedColor;
    }

    public Recipe GetRecipe()
    {
        return _data;
    }
}
