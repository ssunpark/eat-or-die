using TMPro;
using UnityEngine;

public class UI_GameJoin : MonoBehaviour
{
	[SerializeField] private TMP_InputField _inviteCodeInputField;
	
	[SerializeField] private UI_CharacterSelect _characterSelectPopup;

	public void OnClickJoinGame()
	{
		RoomInfoManager.Instance.SetClientGameMode(_inviteCodeInputField.text);
		_characterSelectPopup.Open();
		_characterSelectPopup.Refresh();
	}
}
