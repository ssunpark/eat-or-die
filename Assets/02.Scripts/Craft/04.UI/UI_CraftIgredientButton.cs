using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftIgredientButton : MonoBehaviour
{
    [SerializeField] private Image _craftingredientIcon;
    [SerializeField] private TextMeshProUGUI _craftingredientNameText;
    [SerializeField] private TextMeshProUGUI _currentCountText;
    [SerializeField] private TextMeshProUGUI _isRequiredCountText;

    [SerializeField] private Color _hasEnoughColor = Color.white;
    [SerializeField] private Color _notEnoughColor = Color.red;

    public void Refresh(int ingredientID, int requiredCount)
    {
        var itemProfile = ItemManager.Instance.GetItem(ingredientID);
        _craftingredientIcon.sprite = itemProfile.ItemDefinition.Icon;
        _craftingredientNameText.text = itemProfile.ItemDefinition.Name;

        var currentCount = UnifiedInventoryManager.Instance.GetItemCount(ingredientID);
        _currentCountText.text = currentCount.ToString();
        _isRequiredCountText.text = requiredCount.ToString();
        _currentCountText.color = currentCount >= requiredCount ? _hasEnoughColor : _notEnoughColor;
    }
}