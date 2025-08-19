using TMPro;
using UnityEngine;

public class UI_GameJoin : AUI_PopupBase
{
	public override EPopupType Type => EPopupType.World;
	private bool _isLogin => AuthenticationManager.Instance.User != null;
	
	[SerializeField] private TMP_InputField _inviteCodeInputField;
	[SerializeField] private UI_CharacterSelect _characterSelectPopup;


	public override void Open()
	{
		if (!_isLogin)
		{
			UI_Notification.Notify("로그인이 필요합니다.");
		}
		else
		{
			base.Open();
		}
	}
	
	public void OnClickJoinGame()
	{
		RoomInfoManager.Instance.SetClientGameMode(_inviteCodeInputField.text);
		_characterSelectPopup.Open();
		_characterSelectPopup.Refresh();
	}
}
