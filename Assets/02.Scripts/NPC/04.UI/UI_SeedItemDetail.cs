using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 재배 모종 아이템 세부사항의 정적 데이터 UI
public class UI_SeedItemDetail : MonoBehaviour
{
    [SerializeField] private UI_SeedItemPurchase UI_SeedItemPurchase;
    [Header("아이템 세부 사항 표시")]
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
        
        
        int myGold = 1000; // 임시 골드
        int maxCount = CalcMaxPurchasableCount(npcItem, myGold);
        UI_SeedItemPurchase.Init(itemInfo, npcItem, maxCount);
    }
    
    private int CalcMaxPurchasableCount(NpcItem npcItem, int myGold)
    {
        int stock = npcItem.IsInfinite ? 99 : npcItem.StockQuantity;
        int byGold = myGold / npcItem.Price;
        return Mathf.Clamp(Mathf.Min(stock, byGold), 1, 99);
    }
}