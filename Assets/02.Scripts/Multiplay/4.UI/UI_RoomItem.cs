using TMPro;
using UnityEngine;

public class UI_RoomItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roomNameTextUI;

    public void Refresh(RoomInfoDTO roomInfoDTD)
    {
        _roomNameTextUI.text = roomInfoDTD.RoomName;
    }
}