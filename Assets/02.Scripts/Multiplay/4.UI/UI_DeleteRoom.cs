public class UI_DeleteRoom : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Default;

    private void Awake()
    {
        UI_RoomItem.OnDeleteButtonClicked += Open;
        gameObject.SetActive(false);
    }
    
    public async void DeleteRoom()
    {
        await RoomInfoManager.Instance.DeleteRoom();
        Close();
    }
}