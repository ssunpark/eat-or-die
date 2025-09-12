using UnityEngine;

public class UI_LeaveRoom : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.World;
    
    public void OnClickButtonLeaveRoom()
    {
        RoomInfoNetworkManager.Instance.LeaveRoom();
    }
}