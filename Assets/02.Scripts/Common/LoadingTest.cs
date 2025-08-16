using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [Header("UI")]
    public Slider slider;                 // 로딩바 Slider (min=0, max=1, interactable=false 권장)
    [Tooltip("Handle을 쓰지 않고 직접 스프라이트를 움직이고 싶을 때만 연결")]
    public RectTransform loadingImage;    // (옵션) 직접 이동시킬 스프라이트

    [Header("Direct Move (옵션)")]
    public bool moveSpriteDirectly = false; // true면 Handle 대신 loadingImage를 직접 x로 이동
    public float minX;                      // 바 시작점
    public float maxX;                      // 바 끝점

    void Reset()
    {
        slider = GetComponent<Slider>();
        // 기본값: Handle을 사용 → 자동 이동
        moveSpriteDirectly = false;
        if (slider != null && slider.handleRect != null)
            loadingImage = slider.handleRect; // 안전하게 할당
    }

    void Start()
    {
        // 실행 시 10초 동안 0 → 1로 채우기
        StartCoroutine(Simulate(5f));
    }

    /// <summary>0~1 사이 진행도 설정</summary>
    public void SetProgress(float t)
    {
        t = Mathf.Clamp01(t);
        if (slider != null)
            slider.value = t;

        // Handle 안 쓰고 직접 스프라이트를 이동하고 싶을 때만 사용
        if (moveSpriteDirectly && loadingImage != null)
        {
            float x = Mathf.Lerp(minX, maxX, t);
            var pos = loadingImage.anchoredPosition;
            pos.x = x;
            loadingImage.anchoredPosition = pos;
        }
    }

    /// <summary>duration 동안 0~1 진행도 채우기</summary>
    public IEnumerator Simulate(float duration = 3f)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            SetProgress(t / duration);
            yield return null;
        }
        SetProgress(1f);
    }
}
