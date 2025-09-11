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

        // if (itemDefinition.Type == EItemType.Weapon)
        // {
        //     var extraDescriotion = string.Join("  ", itemDefinition.ExtraDescription);
        //     extraDescriotion = RichTextUtil.RecolorAll(extraDescriotion, "#E44962");
        //     RecipeExtraDescription.text = extraDescriotion;
        //     Debug.Log("무기일 경우에는 상세 설명만 띄운다.");
        // }
        // else
        {
            RecipeDescription.text = itemDefinition.Description;
            // Debug.Log("아닐 경우에는 기존 설명을 띄운다.");
            
            var extraDescriotion = string.Join("  ", itemDefinition.ExtraDescription);
            extraDescriotion = RichTextUtil.RecolorAll(extraDescriotion, "#E44962");
            RecipeExtraDescription.text = extraDescriotion;
            // Debug.Log("무기일 경우에는 상세 설명만 띄운다.");
        }
        
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