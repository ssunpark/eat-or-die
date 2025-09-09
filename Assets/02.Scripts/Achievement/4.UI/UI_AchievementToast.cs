using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UI_AchievementToast : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _stayDuration = 2f;

    private IAchievementPresenter _presenter;
    private CanvasGroup _cg;
    private readonly Queue<AchievementDto> _queue = new();
    private Coroutine _runner;

    void Awake() {
        _presenter = AchievementManager.Instance.Presenter;
        _cg = GetComponent<CanvasGroup>();
        _cg.alpha = 0f; _cg.blocksRaycasts = false; _cg.interactable = false;
    }

    void OnEnable()  => _presenter.OnToast += Enqueue;
    void OnDisable() => _presenter.OnToast -= Enqueue;

    private void Enqueue(AchievementDto dto) {
        _queue.Enqueue(dto);
        if (_runner == null) _runner = StartCoroutine(RunQueue());
    }

    private IEnumerator RunQueue() {
        while (_queue.Count > 0) {
            var dto = _queue.Dequeue();

            if (_title) _title.text = dto.Title ?? "";
            if (_description) _description.text = dto.Description ?? "";

            // Fade In
            yield return Fade(0f, 1f, _fadeDuration);
            yield return new WaitForSeconds(_stayDuration);
            // Fade Out
            yield return Fade(1f, 0f, _fadeDuration);
        }
        _runner = null;
    }

    private IEnumerator Fade(float from, float to, float duration) {
        float t = 0f;
        while (t < duration) {
            t += Time.deltaTime;
            _cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        _cg.alpha = to;
    }
}