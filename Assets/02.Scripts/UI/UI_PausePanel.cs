using TMPro;
using UnityEngine;

public class UI_PausePanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.World;
    
    [Header("방 정보 텍스트")]
    [SerializeField] private TextMeshProUGUI _roomNameText;
    [SerializeField] private TextMeshProUGUI _roomCodeText;
    
    public override void Open()
    {
        base.Open();
        UpdateRoomInfo();
    }
    
    private void UpdateRoomInfo()
    {
         RoomInfo roomInfo = RoomInfoManager.Instance.CurrentRoomInfo;
        
        if (roomInfo != null)
        {
            _roomNameText.text = roomInfo.RoomName;
            
            string inviteCode = RoomInfoManager.Instance.InviteCode;
            if (!string.IsNullOrEmpty(inviteCode))
            {
                _roomCodeText.text = inviteCode;
            }
            else
            {
                _roomCodeText.text = "방 코드 없음";
            }
        }
        else
        {
            _roomNameText.text = "방 정보 없음";
            _roomCodeText.text = "방 코드 없음";
        }
    }
    
    public void OnClickCopyRoomCode()
    {
        string inviteCode = RoomInfoManager.Instance.InviteCode;
        if (!string.IsNullOrEmpty(inviteCode))
        {
            GUIUtility.systemCopyBuffer = inviteCode;
            UI_Notification.Notify("방 코드가 복사되었습니다!");
        }
        else
        {
            UI_Notification.Notify("복사할 방 코드가 없습니다.");
        }
    }
}