using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 수현
public class UI_RecipeButton : MonoBehaviour
{
    // public TextMeshProUGUI RecipeNameTextUI;
    public Image IconImage;
    public Button RecipeButton;
    public Color UnlockedColor = Color.white;
    public Color lockedColor = Color.gray;
    
    private RecipeCSVData _data;
    private bool _isUnlocked = false;
    public int RecipeID => _data.ID;
    public int ResultItemID => _data.ResultID;
    
    public void Refresh(RecipeCSVData Data)
    {
        _data = Data;
        // RecipeNameTextUI.text = Data.Name;
        
        AItemInfo itemInfo = ItemManager.Instance.GetItem(_data.ResultID);
        if (itemInfo != null)
        {
            IconImage.sprite = itemInfo.ItemData.Icon;
            IconImage.gameObject.SetActive(true);
        }
        else
        {
            {
                Debug.LogWarning($"[UI_IngredientButton] 아이템 데이터 없음 - ID: {_data.ID}");
                IconImage.gameObject.SetActive(false);
            }
        }

        // LockButton();
    }

    public void UnlockButton()
    {
        Debug.Log("UnlockButton 메서드 진입");
        _isUnlocked = true;
        RecipeButton.interactable = true;
        IconImage.color = UnlockedColor;
    }
    
    public void LockButton()
    {
        _isUnlocked = false;
        RecipeButton.interactable = false;
        IconImage.color = lockedColor;
    }
    
    public bool IsUnlocked => _isUnlocked;
}
