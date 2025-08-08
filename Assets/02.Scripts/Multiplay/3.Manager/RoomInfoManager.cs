using UnityEngine;

public class RoomInfoManager : NetworkBehaviourSingleton<RoomInfoManager>
{
    [SerializeField] private string _currentSaveKey = "SaveKey"; // 인스펙터에서 키 설정 혹은 유저 로그인 정보 등으로 할당
    public RoomInfo CurrentRoomInfo { get; private set; }
    private RoomInfoRepository _roomInfoRepository =  new RoomInfoRepository();

    private void Awake()
    {
        InitializeRoomInfo();
    }

    private void InitializeRoomInfo()
    {
        bool loaded = _roomInfoRepository.TryLoad(_currentSaveKey, out RoomInfo info); // 저장된 방 정보 있으면 그대로 Load
        CurrentRoomInfo = info;

        if (!loaded) // PlayerPrefs에 저장된 방 정보 없으면 새로 만들고 저장
        {
            Save();
        }
        
        Debug.Log($"[RoomInfoManager] 방 로드됨. 방 이름 : {CurrentRoomInfo.RoomName}");
    }

    public void Save()
    {
        _roomInfoRepository.Save(CurrentRoomInfo, _currentSaveKey);
        Debug.Log("현재 방 정보 저장됨.");
    }

}