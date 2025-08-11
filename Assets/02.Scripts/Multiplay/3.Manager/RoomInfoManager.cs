using Fusion;
using UnityEngine;

public class RoomInfoManager : NetworkBehaviourSingleton<RoomInfoManager>
{
    [SerializeField] private string _persistentRoomID; // 추후 firebase 자동 id로 변경, FirestoreDocumentId
    public RoomInfo CurrentRoomInfo { get; private set; } // 동기화 되어야 함
    private RoomInfoRepository _roomInfoRepository;

    public override void Spawned()
    {
        // '상태 권한'이 있는지(즉, 내가 방장인지) 확인
        if (Object.HasStateAuthority)
        {
            _roomInfoRepository = new RoomInfoRepository();
            InitializeRoomInfo();
        }
    }
    
    private void InitializeRoomInfo()
    {
        var loaded = _roomInfoRepository.TryLoad(_persistentRoomID, out var info); // 저장된 방 정보 있으면 그대로 Load
        CurrentRoomInfo = info;

        if (loaded) // 불러와짐(로드됨)
        {
        }
        else
        {
            CurrentRoomInfo.RoomName = "Room" + _persistentRoomID;
        }

        Debug.Log($"[RoomInfoManager] 방 로드됨. 방 이름 : {CurrentRoomInfo.RoomName}");
    }

    public void Save()
    {
        _roomInfoRepository.Save(_persistentRoomID, CurrentRoomInfo.ToDTO());
        Debug.Log("현재 방 정보 저장됨.");
    }

    // 이 RPC를 OnPlayerJoined에서 호출함
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SyncRoomInfoToNewPlayer(PlayerRef player, string roomInfoJson)
    {
        // 이 RPC는 호스트가 호출하고, 'player' 타겟에게만 전송됩니다.
        Debug.Log("클라이언트가 호스트로부터 RoomInfo를 수신했습니다.");
        var dto = JsonUtility.FromJson<RoomInfoDTO>(roomInfoJson);
        CurrentRoomInfo = dto.ToDomain();
        Debug.Log($"[RoomInfoManager] 동기화 완료. 방 이름: {CurrentRoomInfo.RoomName}");
    }

}