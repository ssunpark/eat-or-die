using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UI_AchievementToast : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI _description;
    [SerializeField]
    private TextMeshProUGUI _currentText;
    [SerializeField]
    private TextMeshProUGUI _targetText;
    [SerializeField]
    private RectTransform _line;
    [SerializeField]
    private float _fadeDuration = 0.5f;
    [SerializeField]
    private float _stayDuration = 2f;

    private IAchievementPresenter _presenter;
    private CanvasGroup _cg;
    private readonly Queue<AchievementViewModel> _queue = new();
    private Coroutine _runner;

    private Vector3 _defaultLineScale = new Vector3(0f, 1f, 1f);

    void Awake()
    {
        _presenter = AchievementManager.Instance.Presenter;
        _cg = GetComponent<CanvasGroup>();
        _cg.alpha = 0f;
        _cg.blocksRaycasts = false;
        _cg.interactable = false;
    }

    void OnEnable() => _presenter.OnToast += Enqueue;
    void OnDisable() => _presenter.OnToast -= Enqueue;

    private void Enqueue(AchievementViewModel viewModel)
    {
        _queue.Enqueue(viewModel);
        if (_runner == null)
            _runner = StartCoroutine(RunQueue());
    }

    private IEnumerator RunQueue()
    {
        while (_queue.Count > 0)
        {
            _line.localScale = _defaultLineScale;
            var dto = _queue.Dequeue();

            _title.text = dto.Title ?? "업적 이름이 없습니다.";
            _description.text = dto.Description ?? "업적 설명이 없습니다.";
            _currentText.text = dto.Current.ToString() ?? "1";
            _targetText.text = dto.Target.ToString() ?? "1";

            // Fade In
            yield return Fade(0f, 1f, _fadeDuration);
            _line.DOScaleX(1, _stayDuration / 2f);
            yield return new WaitForSeconds(_stayDuration);
            // Fade Out
            yield return Fade(1f, 0f, _fadeDuration);
        }

        _runner = null;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            _cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        _cg.alpha = to;
    }
}