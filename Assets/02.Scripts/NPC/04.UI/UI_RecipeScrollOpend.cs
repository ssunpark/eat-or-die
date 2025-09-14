using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_RecipeScrollOpend : MonoBehaviour
{
        public CanvasGroup CanvasGroup;
    public Image RecipeIcon;
    public TextMeshProUGUI RecipeName;
    public TextMeshProUGUI RecipeDescription;
    public TextMeshProUGUI RecipeExtraDescription;
    [SerializeField] private RectTransform panelRectTransform;

    public float displayDuration = 2f;
    public float FadeDuration = 0.4f;
    private Coroutine _animationCoroutine;

    private void OnEnable()
    {
        RecipeShopEvents.OnRecipeScrollUsed += ShowPopup;
        Hide();
    }
    private void ShowPopup(int recipeID)
    {
        ItemProfile itemProfile = ItemManager.Instance.GetItem(recipeID);

        if (itemProfile == null || itemProfile.ItemDefinition == null)
        {
            Debug.LogWarning($"[UI_RecipeScrollOpend] ID({recipeID})에 해당하는 아이템 정보를 찾을 수 없습니다.");
            return;
        }

        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
        
        Refresh(itemProfile.ItemDefinition);

        _animationCoroutine = StartCoroutine(PopupAnimationCoroutine());
    }
    
    private IEnumerator PopupAnimationCoroutine()
    {
        CanvasGroup.alpha = 0f;
        yield return null;

        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRectTransform);
        FadeIn();
        
        yield return new WaitForSeconds(displayDuration);
        FadeOut();
    }
    
    public void Refresh(ItemDefinition itemDefinition)
    {
        if (itemDefinition == null) return;

        RecipeIcon.sprite = itemDefinition.Icon;
        RecipeName.text = itemDefinition.Name;
        RecipeDescription.text = itemDefinition.Description;
        
        var extraDescription = string.Join("  ", itemDefinition.ExtraDescription);
        extraDescription = RichTextUtil.RecolorAll(extraDescription, "#E44962");
        RecipeExtraDescription.text = extraDescription;
    }
    
    private void FadeIn()
    {
        CanvasGroup.DOFade(1f, FadeDuration);
    }

    private void Hide()
    {
        CanvasGroup.alpha = 0f;
    }

    private void FadeOut()
    {
        CanvasGroup.DOFade(0f, FadeDuration).OnComplete(Hide);
    }
}