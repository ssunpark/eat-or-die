public class UI_DeleteRoom : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Default;

    public async void DeleteRoom()
    {
        // await RoomInfoManager.Instance.DeleteRoom(); // 삭제하려는 방의 정보를 언제 넘기느 게 좋을지 고민하고 진행
    }
}