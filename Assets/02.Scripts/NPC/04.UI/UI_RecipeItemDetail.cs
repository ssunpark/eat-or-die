using TMPro;
using UnityEngine;

public class UI_RecipeItemDetail : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI PriceText;

    public void SetDetail(ItemProfile selected, NpcItem npcItem)
    {
        int itemID = selected.ItemDefinition.ID;
        NameText.text = selected.ItemDefinition.Name;
        DescriptionText.text = selected.ItemDefinition.Description;
        PriceText.text = $"구매가격: {npcItem.Price} 골드      <sprite name=Coin>";
    }
}
