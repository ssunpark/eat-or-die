using UnityEngine;

public abstract class AUI_PopupBase : MonoBehaviour
{
	public abstract EPopupType Type { get; }

	public virtual void Open() => gameObject.SetActive(true);
	public virtual void Close() => gameObject.SetActive(false);

	protected virtual void OnEnable()
	{
		Debug.Log($"{Type}: OnEnable");
		PopupManager.Instance?.Register(this);
	}

	protected virtual void OnDisable()
	{
		Debug.Log($"{Type}: OnDisable");
		PopupManager.Instance?.Unregister(this);
	}
}
