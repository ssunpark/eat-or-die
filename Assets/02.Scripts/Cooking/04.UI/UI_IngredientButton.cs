using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 수현
public class UI_IngredientButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // public TextMeshProUGUI IngredientNameTextUI;
    public Image IconImage;
    public Button IngredientButton;
    public Color UnlockedColor = Color.white;
    public Color lockedColor = Color.gray;
    public Sprite unknowIcon;
    private ItemDefinition _data;
    public int IngredientID => _data.ID;

    private void Start()
    {
        IngredientButton.onClick.AddListener(OnClickButton);
        // IconImage.gameObject.SetActive(false);
    }
    
    public void Refresh(ItemDefinition itemDefinition)
    {
        _data = itemDefinition;
        
        if (itemDefinition == null)
        {
            Debug.Log("[UI_IngredientButton] 전달된 아이템 정보가 null입니다.");
            IconImage.gameObject.SetActive(false);
            return;
        }

        var isKnown = RecipePanelUIManager.Instance.IsKnownIngredient(IngredientID);

        if (!isKnown)
        {
            IconImage.sprite = unknowIcon;
            IconImage.color = lockedColor;
            IconImage.gameObject.SetActive(true);
            LockButton();
            return;
        }
        else
        {
            IconImage.sprite = itemDefinition.Icon;
            IconImage.gameObject.SetActive(true);
            UnlockButton();
        }
    }

    public void UnlockButton()
    {
        IngredientButton.interactable = true;
        IconImage.color = UnlockedColor;
    }

    public void LockButton()
    {
        IngredientButton.interactable = false;
        IconImage.color = lockedColor;
    }

    public void OnClickButton()
    {
        RecipePanelUIManager.Instance.SetCurrentIngredientID(IngredientID);
        RecipePanelUIManager.Instance.UpdateRecipes();
    }

    // public void OnClickAllCategoryButton()
    // {
    //     RecipePanelUIManager.Instance.UpdateAllRecipes();
    // }

    public ItemDefinition GetIngredient()
    {
        return _data;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");

        if (_data == null || !RecipePanelUIManager.Instance.IsKnownIngredient(IngredientID))
        {
            return;
        }

        var itemProfile = ItemManager.Instance.GetItem(IngredientID);
        if (itemProfile == null)
        {
            return;
        }

        var sb = new StringBuilder();

        sb.Append($"<b>{itemProfile.ItemDefinition.Name}</b>");
        CookTooltipManager.Instance.Show(sb.ToString());
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit");
        CookTooltipManager.Instance.Hide();
    }
}
