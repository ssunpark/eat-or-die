using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CookingCompletedPopup : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public Image RecipeIcon;
    public TextMeshProUGUI RecipeName;
    public TextMeshProUGUI RecipeDescription;
    public TextMeshProUGUI RecipeExtraDescription;
    [SerializeField] private RectTransform panelRectTransform;

    public float displayDuration = 2f;
    public float FadeDuration = 0.4f;
    private Coroutine _coroutine;

    private void OnEnable()
    {
        CookingManager.Instance.OnCompletedPopupStarted += ShowPopup;
        Hide();
    }

    private void OnDisable()
    {
        if(CookingManager.Instance != null) CookingManager.Instance.OnCompletedPopupStarted -= ShowPopup;
    }

    private void ShowPopup(ItemInstance itemInstance)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(ShowPopupSequence(itemInstance));
    }

    private IEnumerator ShowPopupSequence(ItemInstance itemInstance)
    {
        if (itemInstance == null || itemInstance.ItemProfile.ItemDefinition == null)
        {
            Debug.LogWarning("[UI_CookingCompletedPopup] 전달된 아이템이 null입니다.");
            yield break;
        }

        CanvasGroup.alpha = 0f;
        Refresh(itemInstance.ItemProfile.ItemDefinition);
        yield return null;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRectTransform);
        FadeIn();
        
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(HideAfterDelay());
    }
    
    public void Refresh(ItemDefinition itemDefinition)
    {
        if (itemDefinition == null)
        {
            Debug.Log("[UICookingCompletedPopup] itemData is null");
            return;
        }

        RecipeIcon.sprite = itemDefinition.Icon;
        RecipeName.text = itemDefinition.Name;
        RecipeDescription.text = itemDefinition.Description;
        
        var extraDescriotion = string.Join("  ", itemDefinition.ExtraDescription);
        extraDescriotion = RichTextUtil.RecolorAll(extraDescriotion, "#E44962");
        RecipeExtraDescription.text = extraDescriotion;
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
        CanvasGroup.DOFade(0f, FadeDuration);
    }
    
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        FadeOut();
    }
}