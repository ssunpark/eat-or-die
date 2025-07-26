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
    public TextMeshProUGUI OwnedCountText;

    public void SetDetail(AItemInfo itemInfo, NpcItem npcItem)
    {
        int itemID = itemInfo.ItemData.ID;
        int count = InventoryManager.Instance.Inventory.GetItemCount(itemID);
        OwnedCountText.text = $"소지개수: {count.ToString()} 개";
        
        IconImage.sprite = itemInfo.ItemData.Icon;
        NameText.text = itemInfo.ItemData.Name;
        DescriptionText.text = itemInfo.ItemData.Description;

        PriceText.text = $"구매가격: {npcItem.Price} 골드";
        StockText.text = npcItem.IsInfinite ? "재고수량: 무한" : $"재고수량: {npcItem.StockQuantity}";
    }
}