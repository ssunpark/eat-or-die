using DG.Tweening;
using UnityEngine;

public class UI_PopupAnimator : MonoBehaviour
{
    public enum PopupEffectType
    {
        Scale,
        Fade,
        Slide,
        ScaleAndFade
    }

    public enum SlideDirection
    {
        FromBottom,
        FromTop,
        FromLeft,
        FromRight
    }

    [Header("애니메이션 타입 선택")] public PopupEffectType effectType = PopupEffectType.Scale;

    [Header("테스트용 토글 (창 열기)")] public bool testOpen;

    [Header("설정")] public float duration = 0.3f;
    public Ease ease = Ease.OutBack;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    public SlideDirection slideDirection = SlideDirection.FromBottom; // 기본값은 아래에서

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        PlayStartAnimation();
    }

    private void Update()
    {
        // 에디터에서 테스트하기 쉽게
        if (testOpen)
        {
            testOpen = false;
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayCloseAnimation();
        }
    }

    public void PlayStartAnimation()
    {
        // 초기화
        transform.localScale = Vector3.one;
        canvasGroup.alpha = 1;
        rectTransform.anchoredPosition = Vector2.zero;

        switch (effectType)
        {
            case PopupEffectType.Scale:
                transform.localScale = Vector3.zero;
                transform.DOScale(Vector3.one, duration).SetEase(ease);
                break;

            case PopupEffectType.Fade:
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1, duration).SetEase(Ease.Linear);
                break;

            case PopupEffectType.Slide:
                var startPos = Vector2.zero;

                switch (slideDirection)
                {
                    case SlideDirection.FromBottom:
                        startPos = new Vector2(0, -Screen.height);
                        break;
                    case SlideDirection.FromTop:
                        startPos = new Vector2(0, Screen.height);
                        break;
                    case SlideDirection.FromLeft:
                        startPos = new Vector2(-Screen.width, 0);
                        break;
                    case SlideDirection.FromRight:
                        startPos = new Vector2(Screen.width, 0);
                        break;
                }

                rectTransform.anchoredPosition = startPos;
                rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutCubic);
                break;

            case PopupEffectType.ScaleAndFade:
                transform.localScale = Vector3.zero;
                canvasGroup.alpha = 0;
                var seq = DOTween.Sequence();
                seq.Append(transform.DOScale(Vector3.one, duration).SetEase(ease));
                seq.Join(canvasGroup.DOFade(1, duration * 0.8f));
                break;
        }
    }

    public void PlayCloseAnimation()
    {
        // DOTween 시퀀스/트윈이 실행 중일 수 있으므로 모두 죽이고 시작
        transform.DOKill();
        canvasGroup.DOKill();

        switch (effectType)
        {
            case PopupEffectType.Scale:
                transform.DOScale(Vector3.zero, duration)
                    .SetEase(ease)
                    .OnComplete(() => gameObject.SetActive(false)); // 애니메이션 끝나면 비활성화
                break;

            case PopupEffectType.Fade:
                canvasGroup.DOFade(0, duration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => gameObject.SetActive(false));
                break;

            case PopupEffectType.Slide:
                var endPos = Vector2.zero;
                switch (slideDirection)
                {
                    case SlideDirection.FromBottom: endPos = new Vector2(0, -Screen.height); break;
                    case SlideDirection.FromTop: endPos = new Vector2(0, Screen.height); break;
                    case SlideDirection.FromLeft: endPos = new Vector2(-Screen.width, 0); break;
                    case SlideDirection.FromRight: endPos = new Vector2(Screen.width, 0); break;
                }

                rectTransform.DOAnchorPos(endPos, duration)
                    .SetEase(Ease.InCubic) // 들어올 때 Out이면 나갈 땐 In을 쓰는게 자연스러움
                    .OnComplete(() => gameObject.SetActive(false));
                break;

            case PopupEffectType.ScaleAndFade:
                var seq = DOTween.Sequence();
                seq.Append(transform.DOScale(Vector3.zero, duration).SetEase(ease));
                seq.Join(canvasGroup.DOFade(0, duration * 0.8f));
                seq.OnComplete(() => gameObject.SetActive(false));
                break;
        }
    }
}