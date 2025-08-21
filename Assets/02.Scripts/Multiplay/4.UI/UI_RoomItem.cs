using System;
using TMPro;
using UnityEngine;

public class UI_RoomItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roomNameTextUI;
    private RoomInfoDTO _roomInfoDTO;
    public static event Action OnDeleteButtonClicked;

    private void Start()
    {
        RoomInfoManager.Instance.OnRoomInfoUpdated += CreateInviteCode;
    }
    
    public void Refresh(RoomInfoDTO roomInfoDTD)
    {
        _roomInfoDTO = roomInfoDTD;
        _roomNameTextUI.text = roomInfoDTD.RoomName;
    }

    public void OnClickRoom()
    {
        RoomInfoManager.Instance.SetRoomInfoDTO(_roomInfoDTO);
    }

    public async void CreateInviteCode()
    {
        string code = await RoomInfoManager.Instance.GenerateInviteCode();

        if (!string.IsNullOrEmpty(code))
        {
            GUIUtility.systemCopyBuffer = code;
            UI_Notification.Notify("초대 코드가 복사되었습니다.");
        }
        else
        {
            Debug.Log("초대 코드 생성 실패");
            UI_Notification.Notify("초대 코드 생성에 실패했습니다.");
        }
    }

    public void OnClickDeleteButton()
    {
        RoomInfoManager.Instance.SetDeleteRoom(_roomInfoDTO);
        OnDeleteButtonClicked?.Invoke();
    }
}