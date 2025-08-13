using System;
using UnityEngine;

public abstract class AUI_PopupBase : MonoBehaviour
{
	public abstract EPopupType Type { get; }

	private AnimatePopup _animatePopup;
    public event Action<AUI_PopupBase> Opened;
    public event Action<AUI_PopupBase> Closed;
    public virtual void Open()
	{
		if (gameObject.activeInHierarchy) return;
		
		gameObject.SetActive(true);
		_animatePopup?.Open();
		PopupManager.Instance.Register(this);
        Opened?.Invoke(this);
    }
	
	public virtual void Close()
	{
		if (!gameObject.activeInHierarchy) return;
        PopupManager.Instance.Unregister(this);
		
		if (_animatePopup == null)
		{
			gameObject.SetActive(false);
		}
		else
		{
			_animatePopup?.Close();
		}
        Closed?.Invoke(this);
    }

	protected virtual void Awake()
	{
		_animatePopup = GetComponent<AnimatePopup>();
	}
}
