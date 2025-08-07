using UnityEngine;

public class RoomInfoManager : NetworkBehaviourSingleton<RoomInfoManager>
{
    public RoomInfo CurrentRoomInfo { get; private set; }
    private RoomInfoRepository _roomInfoRepository =  new RoomInfoRepository();

    private void Awake()
    {
        CurrentRoomInfo = _roomInfoRepository.Load();
        Debug.Log($"[RoomInfoManager] 방 로드됨. 방 이름 : {CurrentRoomInfo.RoomName}");
    }

    public void Save()
    {
        _roomInfoRepository.Save(CurrentRoomInfo);
        Debug.Log("현재 방 정보 저장됨.");
    }

}