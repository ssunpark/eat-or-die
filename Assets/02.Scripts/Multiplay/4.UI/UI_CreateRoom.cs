using TMPro;
using UnityEngine;

public class UI_CreateRoom : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.World;

    [SerializeField] private TMP_InputField _roomNameTextField;

    public async void CreateRoom()
    {
        if (string.IsNullOrEmpty(_roomNameTextField.text))
        {
            UI_Notification.Notify("방 이름을 입력하세요.");
            return;
        }

        var roomInfo = new RoomInfo(_roomNameTextField.text);
        var dto = roomInfo.ToDTO();

        await RoomInfoManager.Instance.CreateRoom(dto);
        Close();
    }
}