using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RecipeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
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
        if (_data == null || !RecipePanelUIManager.Instance.IsKnownRecipe(_data.ID))
        {
            return;
        }

        var itemProfile = ItemManager.Instance.GetItem(_data.ResultID);
        if (itemProfile == null)
        {
            return;
        }

        var sb = new StringBuilder();

        sb.Append($"<color=#7BD9B2><b>{itemProfile.ItemDefinition.Name}</b></color>\n\n");
        sb.Append($"{itemProfile.ItemDefinition.Description}\n\n"); // 설명

        var extraDescription = string.Join("  ", itemProfile.ItemDefinition.ExtraDescription);
        extraDescription = RichTextUtil.RecolorAll(extraDescription, "#E44962");
        sb.Append($"{extraDescription}");

        TooltipManager.Instance.Show(sb.ToString());
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.Hide();
    }
}
