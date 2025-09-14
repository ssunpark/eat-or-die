using DarkTonic.MasterAudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RecipeItemDetail : MonoBehaviour
{
    [Header("Item Info")]
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI PriceText;

    [Header("Currency & Purchase")]
    public TextMeshProUGUI CurrentGoldText;
    public Button PurchaseButton;

    private ItemProfile _currentItem;
    private NpcItem _currentNpcItem;

    private void Start()
    {
        CurrencyManager.Instance.OnCurrencyChanged += OnCurrencyChanged;
        if (PurchaseButton != null)
        {
            PurchaseButton.onClick.AddListener(OnPurchaseClicked);
        }
    }

    private void OnEnable()
    {
        UpdateCurrentGoldDisplay();
    }

    public void SetDetail(ItemProfile selected, NpcItem npcItem)
    {
        _currentItem = selected;
        _currentNpcItem = npcItem;

        int itemID = selected.ItemDefinition.ID;
        NameText.text = selected.ItemDefinition.Name;
        DescriptionText.text = selected.ItemDefinition.Description;
        PriceText.text = $"구매가격: {npcItem.Price} 골드      <sprite name=Coin>";

        UpdateCurrentGoldDisplay();
        UpdatePurchaseButton();
    }

    private void OnCurrencyChanged(ECurrencyType currencyType, int newAmount)
    {
        if (currencyType == ECurrencyType.Gold)
        {
            UpdateCurrentGoldDisplay();
            UpdatePurchaseButton();
        }
    }

    private void UpdateCurrentGoldDisplay()
    {
        if (CurrentGoldText != null && CurrencyManager.Instance != null)
        {
            int currentGold = CurrencyManager.Instance.Get(ECurrencyType.Gold);
            CurrentGoldText.text = $"{currentGold}      <sprite name=Coin>";
        }
    }

    private void UpdatePurchaseButton()
    {
        if (PurchaseButton == null || _currentNpcItem == null) return;

        int currentGold = CurrencyManager.Instance.Get(ECurrencyType.Gold);
        bool canAfford = currentGold >= _currentNpcItem.Price;

        PurchaseButton.interactable = canAfford;
    }

    private void OnPurchaseClicked()
    {
        if (_currentItem == null || _currentNpcItem == null)
        {
            return;
        }

        int price = _currentNpcItem.Price;

        // 골드 차감 시도
        if (!CurrencyManager.Instance.TrySpend(ECurrencyType.Gold, price))
        {
            Debug.Log($"[RecipeDetail] 골드가 부족합니다. 필요: {price}, 보유: {CurrencyManager.Instance.Get(ECurrencyType.Gold)}");
            return;
        }
        
        const int recipeScrollItemID = 500003;
        ItemProfile scrollItemProfile = ItemManager.Instance.GetItem(recipeScrollItemID);

        if (scrollItemProfile == null)
        {
            Debug.LogError($"[RecipeDetail] 아이템 ID({recipeScrollItemID})를 찾을 수 없습니다.");
            // (중요) 골드를 다시 돌려주는 로직이 필요할 수 있습니다.
            CurrencyManager.Instance.Add(ECurrencyType.Gold, price); 
            return;
        }

        // 2. 이 스크롤이 해금할 레시피의 ID를 가져와 문자열로 변환합니다.
        int recipeToUnlockID = _currentItem.ItemDefinition.ID;
        string extraInfo = recipeToUnlockID.ToString();

        // 3. ItemInstance를 직접 생성합니다. 생성자의 extraInfo 파라미터에 레시피 ID 문자열을 전달합니다.
        ItemInstance scrollInstance = new ItemInstance(scrollItemProfile, 1, scrollItemProfile.ItemDefinition.MaxDurability, extraInfo);

        // 4. 생성된 ItemInstance를 통합 인벤토리 매니저에 추가합니다.
        UnifiedInventoryManager.Instance.AddItem(scrollInstance);

        // 5. 구매된 레시피 아이템을 RecipeShopManager에 추가
        RecipeShopManager.Instance.OnRecipeItemPurchased(recipeToUnlockID);

        Debug.Log($"[RecipeDetail] 레시피 스크롤 구매 완료: 아이템ID={recipeToUnlockID}, 가격={price}");
    }
}
