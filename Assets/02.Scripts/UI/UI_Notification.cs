using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UI_Notification : BehaviourSingleton<UI_Notification>
{
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private float _fadeDuration = 0.5f; // Fade in/out 속도
    [SerializeField]
    private float _stayDuration = 2f;   // 유지 시간

    private void Awake()
    {
        Instance.gameObject.SetActive(false);
    }

    public static void Notify(string message)
    {
        Instance._text.text = message;
        Instance.Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // Fade in
        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / _fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = 1f;

        // 유지
        yield return new WaitForSeconds(_stayDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / _fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = 0f;

        gameObject.SetActive(false);
    }
}