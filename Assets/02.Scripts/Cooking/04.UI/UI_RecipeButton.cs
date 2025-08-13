using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RecipeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // public TextMeshProUGUI RecipeNameTextUI;
    public Image IconImage;
    public Button RecipeButton;
    public Color UnlockedColor = Color.white;
    public Color lockedColor = Color.gray;
    public Sprite unknownIcon;

    // public GameObject CookingHoverUI;
    // public TextMeshProUGUI RecipeNameText;
    // public TextMeshProUGUI RecipeDescriptionText;
    // public TextMeshProUGUI RecipeExtraDescriptionText;
    
    private Recipe _data;
    public int RecipeID => _data.ID;
    public int ResultItemID => _data.ResultID;
    
    public void Refresh(Recipe Data)
    {
        _data = Data;
        // RecipeNameTextUI.text = Data.Name;
        
        bool isKnown = RecipePanelUIManager.Instance.IsKnownRecipe(_data.ID);
        bool canMake = RecipePanelUIManager.Instance.CanMakeRecipe(_data);
        
        ItemProfile itemProfile = ItemManager.Instance.GetItem(_data.ResultID);

        if (!isKnown) // 방 기준으로 습득되지 않은 레시피에 대해서
        {
            IconImage.sprite = unknownIcon;
            IconImage.color = lockedColor;
            IconImage.gameObject.SetActive(true);
            LockButton();
            return;
        }
        
        if (itemProfile != null)
        {
            IconImage.sprite = itemProfile.ItemDefinition.Icon;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ui 활성화
        Debug.Log("OnPointerEnter");
        
        // 데이터가 없거나, 아직 모르는 레시피라면 툴팁을 띄우지 않음
        if (_data == null || !RecipePanelUIManager.Instance.IsKnownRecipe(_data.ID))
        {
            CookingHoverUI.gameObject.SetActive(false);
        }
        else
        {
            CookingHoverUI.gameObject.SetActive(true);
        }
        // 툴팁에 표시할 결과 아이템의 정보를 가져옴
        var itemProfile = ItemManager.Instance.GetItem(_data.ResultID);
        if (itemProfile == null)
        {
            return;
        }

        RecipeNameText.text = itemProfile.ItemDefinition.Name;
        RecipeDescriptionText.text = itemProfile.ItemDefinition.Description;

        var extraDescriotion = string.Join("  ", itemProfile.ItemDefinition.ExtraDescription);
        extraDescriotion = RichTextUtil.RecolorAll(extraDescriotion, "#E44962");
        RecipeExtraDescriptionText.text = extraDescriotion;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        // ui 비활성화
        Debug.Log("OnPointerExit");
        CookingHoverUI.gameObject.SetActive(false);
    }
}
