using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        CookingManager.CookingFinished += ShowPopup;
        Hide();
    }

    private void OnDisable()
    {
        CookingManager.CookingFinished -= ShowPopup;
    }

    private void ShowPopup(Item item)
    {
        if (item == null || item.ItemInfo.ItemData == null)
        {
            Debug.LogWarning("[UI_CookingCompletedPopup] 전달된 아이템이 null입니다.");
            return;
        }

        Refresh(item.ItemInfo.ItemData);
        FadeIn();

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(HideAfterDelay());

    }
    
    public void Refresh(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.Log("[UICookingCompletedPopup] itemData is null");
            return;
        }

        RecipeIcon.sprite = itemData.Icon;
        RecipeName.text = itemData.Name;
        RecipeDescription.text = itemData.Description;
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