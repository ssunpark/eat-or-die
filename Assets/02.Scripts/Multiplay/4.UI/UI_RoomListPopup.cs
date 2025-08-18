using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomListPopup : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.World;
    public FadeController NeedLoginNotification;
    private bool _isLogin => AuthenticationManager.Instance.User != null;

    [Header("방 생성")] [SerializeField] private GameObject _container;
    [SerializeField] private GameObject _roomPrefab;
    [SerializeField] private UI_CharacterSelect _characterSelectPopup;
    protected override void Awake()
    {
        base.Awake();
        RoomInfoManager.Instance.OnDataChanged += Refresh;
    }

    public override void Open()
    {
        if (!_isLogin)
        {
            if (NeedLoginNotification == null)
            {
                Debug.LogError("NeedLoginNotification is null");
            }

            NeedLoginNotification.FadeIn();
            NeedLoginNotification.FadeOutAfterDelay();
        }
        else
        {
            base.Open();
            Refresh();
        }
    }

    private void Refresh()
    {
        int childCount = _container.transform.childCount;
        
        for (int i = 0; i < childCount; i++)
        {
            Destroy(_container.transform.GetChild(i).gameObject);
        }

        // RoomInfoManager에서 리스트 가져오기
        var roomInfoList = RoomInfoManager.Instance.RoomInfoList;

        if (roomInfoList == null || roomInfoList.Count == 0)
        {
            return;
        }

        for (int i = 0; i < roomInfoList.Count; i++)
        {
            var roomInfo = roomInfoList[i];
            // Prefab 생성
            var roomItem = Instantiate(_roomPrefab, _container.transform);
            Button roomButton = roomItem.GetComponent<Button>();
            roomButton.onClick.AddListener(OpenCharacterSelectPopup);
            // UI 세팅
            var roomUI = roomItem.GetComponent<UI_RoomItem>();
            if (roomUI != null)
            {
                roomUI.Refresh(roomInfo); // RoomInfoDTO 기반 세팅
            }
        }
    }

    private void OpenCharacterSelectPopup()
    {
        RoomInfoManager.Instance.GameMode = GameMode.Host;
        _characterSelectPopup.Open();
        _characterSelectPopup.Refresh();
    }
}