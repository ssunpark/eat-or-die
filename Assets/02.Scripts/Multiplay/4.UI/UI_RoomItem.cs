using TMPro;
using UnityEngine;

public class UI_RoomItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roomNameTextUI;
    private RoomInfo _roomInfo;
    public void Refresh(RoomInfoDTO roomInfoDTD)
    {
        _roomInfo = roomInfoDTD.ToDomain();
        _roomNameTextUI.text = roomInfoDTD.RoomName;
    }

    public void OnClick()
    {
        RoomInfoManager.Instance.CurrentRoomInfo = _roomInfo;
    }
}