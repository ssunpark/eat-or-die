using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopupManager : BehaviourSingleton<PopupManager>
{
	private readonly List<UI_PopupBase> openedPopups = new();
	public bool HasOpenedPopup => openedPopups.Count > 0;

	public void Register(UI_PopupBase popup)
	{
		if (!openedPopups.Contains(popup))
			openedPopups.Add(popup);
	}

	public void Unregister(UI_PopupBase popup)
	{
		openedPopups.Remove(popup);
	}

	public void CloseLast()
	{
		if (openedPopups.Count > 0)
		{
			openedPopups[^1].Close();
		}
	}

	public bool IsOpen(EPopupType type)
	{
		return openedPopups.Any(popup => popup.Type == type);
	}

	public UI_PopupBase GetOpenPopup(EPopupType type)
	{
		return openedPopups.FirstOrDefault(popup => popup.Type == type);
	}

	public IReadOnlyList<UI_PopupBase> GetAllOpenPopups()
	{
		// 열려있는 모든 팝업을 반환하는 메서드
		// 캡슐화를 위해 열려있는 모든 팝업을 닫는 메서드로 변경될 수 있음
		return openedPopups.AsReadOnly();
	}

}
