using UnityEngine;

public abstract class UI_PopupBase : MonoBehaviour
{
	public abstract EPopupType Type { get; }

	public virtual void Open() => gameObject.SetActive(true);
	public virtual void Close() => gameObject.SetActive(false);

	protected virtual void OnEnable()
	{
		Debug.Log("OnEnable");
		PopupManager.Instance.Register(this);
	}

	protected virtual void OnDisable()
	{
		Debug.Log("OnDisable");
		PopupManager.Instance.Unregister(this);
	}
}
