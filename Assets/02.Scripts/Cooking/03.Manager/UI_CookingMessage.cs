using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

// 수현
public class UI_CookingMessage : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public TextMeshProUGUI AlertText;
    public float displayDuration = 3.8f;
    public float FadeDuration = 0.4f;
    private Coroutine _coroutine;

    private void OnEnable()
    {
        CookingManager.Instance.OnAlertMessage += ShowAlert;
        Hide();
    }

    private void OnDisable()
    {
        if(CookingManager.Instance != null) CookingManager.Instance.OnAlertMessage -= ShowAlert;
    }

    public void ShowAlert(string message)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        AlertText.text = message;
        AlertText.rectTransform.DOShakePosition(
            duration: displayDuration,
            strength: new Vector3(0, 10f, 0),
            vibrato: 10,
            randomness: 90,
            snapping: false,
            fadeOut: true
        );
        FadeIn();
        _coroutine = StartCoroutine(HideAfterDelay());
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
