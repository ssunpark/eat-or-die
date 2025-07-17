using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FoodItemBehaviour : MonoBehaviour
{
    public FoodItem FoodItemData { get; private set; }

    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _nameText;

    public void Init(FoodItem foodItem)
    {
        FoodItemData = foodItem;
        _nameText.text = foodItem.ItemData.Name;
        // _iconImage.sprite = foodItem.ItemData.Icon;
    }
}
