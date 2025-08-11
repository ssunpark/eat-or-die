using UnityEngine;

public abstract class AUI_PopupBase : MonoBehaviour
{
	public abstract EPopupType Type { get; }

	private AnimatePopup _animatePopup;

	public virtual void Open()
	{
		gameObject.SetActive(true);
		_animatePopup?.Open();
		PopupManager.Instance.Register(this);
	}
	
	public virtual void Close()
	{
		PopupManager.Instance.Unregister(this);
		
		if (_animatePopup == null)
		{
			gameObject.SetActive(false);
		}
		else
		{
			_animatePopup?.Close();
		}
	}

	protected virtual void Awake()
	{
		_animatePopup = GetComponent<AnimatePopup>();
	}
}
