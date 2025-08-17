using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using Fusion;
using UnityEngine;

public class RoomInfoManager : BehaviourSingleton<RoomInfoManager>
{
    public RoomInfo CurrentRoomInfo;
    private RoomInfoRepository _roomInfoRepository;

    private string _userID => AuthenticationManager.Instance.User.UserId;

    public List<RoomInfoDTO> RoomInfoList { get; private set; }

    public event Action OnDataChanged;

    public async void Awake()
    {
        DontDestroyOnLoad(this);
        await FirebaseManager.Instance.WaitForInitialization();

        // _roomInfoRepository = new RoomInfoRepository(FirebaseManager.Instance.DB);
        _roomInfoRepository = new RoomInfoRepository(FirebaseFirestore.DefaultInstance);
        AuthenticationManager.Instance.OnLogin += InitializeRoomInfos;
    }

    private async void InitializeRoomInfos()
    {
        try
        {
            RoomInfoList = await _roomInfoRepository.GetRoomInfosByUserId(_userID);
            OnDataChanged?.Invoke();
            Debug.Log($"[RoomInfoManager] {RoomInfoList.Count}개의 방 정보 로드 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[RoomInfoManager] 방 정보 초기화 실패: {e.Message}");
        }
    }

    public async UniTask Save()
    {
        if (CurrentRoomInfo == null)
        {
            return;
        }

        try
        {
            await _roomInfoRepository.UpdateRoomInfo(CurrentRoomInfo.ToDTO(), _userID);

            Debug.Log("[RoomInfoManager] 현재 방 정보 저장됨.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[RoomInfoManager] 방 정보 저장 실패: {e.Message}");
        }
    }

    public async UniTask CreateRoom(RoomInfoDTO roomInfoDTO)
    {
        Debug.Log(_roomInfoRepository);
        try
        {
            await _roomInfoRepository.AddRoomInfo(roomInfoDTO, _userID);
            InitializeRoomInfos();
            Debug.Log("[RoomInfoManager] 현재 방 정보 저장됨.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[RoomInfoManager] 방 정보 저장 실패: {e.Message}");
        }
    }

    // 새 플레이어 동기화용 RPC
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SyncRoomInfoToNewPlayer(PlayerRef player, string roomInfoJson)
    {
        Debug.Log("클라이언트가 호스트로부터 RoomInfo를 수신했습니다.");
        CurrentRoomInfo = JsonUtility.FromJson<RoomInfo>(roomInfoJson);
        Debug.Log($"[RoomInfoManager] 동기화 완료. 방 이름: {CurrentRoomInfo.RoomName}");
    }
}
