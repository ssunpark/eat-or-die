using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : BehaviourSingleton<TooltipManager>
{
    public CanvasGroup canvasGroup;
    public UI_TooltipPanel tooltipPanel;
    public float fadeTime = 0.15f;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (tooltipPanel != null)
        {
            tooltipPanel.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (tooltipPanel != null && tooltipPanel.gameObject.activeSelf)
        {
            tooltipPanel.transform.position = Input.mousePosition;
        }
    }

    public void Show(string content)
    {
        Show(content, true); // 기본값으로 유동적 width 사용
    }
    
    public void Show(string content, bool useFlexibleWidth)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        tooltipPanel.gameObject.SetActive(true);
        tooltipPanel.SetText(content, useFlexibleWidth);

        _fadeCoroutine = StartCoroutine(Fade(1f));
    }

    public void Hide()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        var startAlpha = canvasGroup.alpha;
        var timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (targetAlpha == 0f)
        {
            tooltipPanel.gameObject.SetActive(false);
        }
    }
}
