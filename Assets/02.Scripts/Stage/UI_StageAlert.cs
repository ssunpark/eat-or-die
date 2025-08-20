using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class UI_StageAlert : MonoBehaviour
{
	[SerializeField] private CanvasGroup _canvasGroup;
	[SerializeField] private TextMeshProUGUI _stageNameText;
	[SerializeField] private float _fadeDuration = 1f;
	[SerializeField] private float _displayDuration = 2f;
	
	private void Awake()
	{
		StageManager.Instance.OnStageAlert += ShowAlert;
		gameObject.SetActive(false);
	}

	private void ShowAlert(string stageName)
	{
		_stageNameText.text = stageName;

		_canvasGroup.alpha = 0f;
		
		gameObject.SetActive(true);
		
		_canvasGroup.DOFade(1f, _fadeDuration).OnComplete(() =>
		{
			StartCoroutine(FadeOutCoroutine());
		});
	}
	
	private IEnumerator FadeOutCoroutine()
	{
		yield return new WaitForSeconds(_displayDuration);
		
		_canvasGroup.DOFade(0f, _fadeDuration).OnComplete(() =>
		{
			gameObject.SetActive(false);
		});
	}
}
