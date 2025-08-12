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
        CookingManager.Instance.OnCompletedPopupStarted -= ShowPopup;
    }

    private void ShowPopup(ItemInstance itemInstance)
    {
        if (itemInstance == null || itemInstance.ItemProfile.ItemDefinition == null)
        {
            Debug.LogWarning("[UI_CookingCompletedPopup] 전달된 아이템이 null입니다.");
            return;
        }

        Refresh(itemInstance.ItemProfile.ItemDefinition);
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