using System.Text;
using DarkTonic.MasterAudio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public ItemDefinition GetIngredient()
    {
        return _data;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
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
        TooltipManager.Instance.Show(sb.ToString());
        MasterAudio.PlaySound("ButtonClick");
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.Hide();
    }
}
