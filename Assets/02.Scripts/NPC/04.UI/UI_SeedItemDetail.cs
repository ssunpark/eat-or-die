using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_SeedItemDetail : MonoBehaviour
{
    public Image IconImage;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI PriceText;
    public TextMeshProUGUI StockText;

    public void SetDetail(AItemInfo itemInfo, NpcItem npcItem)
    {
        IconImage.sprite = itemInfo.ItemData.Icon;
        NameText.text = itemInfo.ItemData.Name;
        DescriptionText.text = itemInfo.ItemData.Description;

        PriceText.text = $"구매가격: {npcItem.Price} 골드";
        StockText.text = npcItem.IsInfinite ? "재고수량: 무한" : $"재고수량: {npcItem.StockQuantity}";
    }
}