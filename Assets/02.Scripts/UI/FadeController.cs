using System.Collections;
using UnityEngine;

// 스크립트를 여러 GameObject에 재사용할 수 있도록 제네릭하게 만듭니다.
public class FadeController : MonoBehaviour
{
    // Inspector에서 조절 가능한 공통 변수들
    [Header("Fade Settings")]
    [Tooltip("페이드 인/아웃에 걸리는 시간 (초)")]
    public float fadeDuration = 1.0f;
    [Tooltip("오브젝트가 투명하게 변하는 시간 (초)")]
    public float hideDuration = 1.0f;
    [Tooltip("페이드 시작 시의 초기 색상")]
    public Color startColor = Color.white;
    [Tooltip("페이드 완료 시의 목표 색상")]
    public Color endColor = Color.white;

    private Coroutine _fadeCoroutine;

    // Fade 대상 (Renderer 또는 CanvasGroup)
    private Renderer _renderer;
    private CanvasGroup _canvasGroup;

    // 스크립트 시작 시 Fade 대상을 미리 캐싱
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    // 페이드 인 코루틴
    public void FadeIn()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(DoFade(1.0f, startColor, endColor));
    }

    // 페이드 아웃 코루틴
    public void FadeOut()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(DoFade(0.0f, endColor, startColor));
    }

    // 지정된 시간 이후에 페이드 아웃
    public void FadeOutAfterDelay()
    {
        // if (_fadeCoroutine != null)
        // {
        //     StopCoroutine(_fadeCoroutine);
        // }
        _fadeCoroutine = StartCoroutine(HideAfterDelay(hideDuration));
    }

    // Fade 코루틴 (Lerp를 사용하여 부드러운 전환)
    private IEnumerator DoFade(float targetAlpha, Color startColor, Color endColor)
    {
        float timer = 0.0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;
            float alpha = Mathf.Lerp(1 - targetAlpha, targetAlpha, progress);

            if (_renderer != null)
            {
                // 3D 오브젝트의 머티리얼 색상 변경
                Color newColor = Color.Lerp(startColor, endColor, progress);
                newColor.a = alpha;
                _renderer.material.color = newColor;
            }
            else if (_canvasGroup != null)
            {
                // UI CanvasGroup의 알파값 변경
                _canvasGroup.alpha = alpha;
            }

            yield return null;
        }

        // 코루틴 완료 후 최종 알파값 설정
        if (_renderer != null)
        {
            Color finalColor = Color.Lerp(startColor, endColor, 1.0f);
            finalColor.a = targetAlpha;
            _renderer.material.color = finalColor;
        }
        else if (_canvasGroup != null)
        {
            _canvasGroup.alpha = targetAlpha;
        }

        _fadeCoroutine = null;
    }

    // 지정된 시간만큼 기다린 후 FadeOut 실행
    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        FadeOut();
    }
}