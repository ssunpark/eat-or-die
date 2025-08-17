using TMPro;
using UnityEngine;

public class UI_RoomItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roomNameTextUI;
    private RoomInfoDTO _roomInfoDTO;
    public void Refresh(RoomInfoDTO roomInfoDTD)
    {
        _roomInfoDTO = roomInfoDTD;
        _roomNameTextUI.text = roomInfoDTD.RoomName;
    }

    public void OnClick()
    {
        RoomInfoManager.Instance.SetRoomInfoDTO(_roomInfoDTO);
    }
}