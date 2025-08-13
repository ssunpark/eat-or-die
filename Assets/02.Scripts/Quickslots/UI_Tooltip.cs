using Ricimi;
using DG.Tweening;
using UnityEngine;

public class UI_Tooltip : MonoBehaviour
{
	public GameObject tooltip;

	public float fadeTime = 0.1f;

	public void OnPointerEnter()
	{
		if (tooltip != null)
		{
			tooltip.GetComponent<CanvasGroup>().DOFade(1.0f, fadeTime).SetEase(Ease.InOutQuad);
		}
	}

	public void OnPointerExit()
	{
		if (tooltip != null)
		{
			tooltip.GetComponent<CanvasGroup>().DOFade(0.0f, fadeTime).SetEase(Ease.InOutQuad);
		}
	}
}
	
