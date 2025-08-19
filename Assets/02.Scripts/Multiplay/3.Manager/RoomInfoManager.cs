using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using Fusion;
using UnityEngine;

public class RoomInfoManager : BehaviourSingleton<RoomInfoManager>
{
    public RoomInfo CurrentRoomInfo; //{ get; private set; }
    public RoomInfoDTO CurrentRoomInfoDTO { get; private set; }
    private RoomInfoRepository _roomInfoRepository;
    private string _userID => AuthenticationManager.Instance.User.UserId;

    public List<RoomInfoDTO> RoomInfoList { get; private set; }
    public event Action OnDataChanged;
    public event Action OnCurrentRoomInfoUpdated;
    public string InviteCode;
    public GameMode GameMode; // 임시 코드

    public async void Awake()
    {
        DontDestroyOnLoad(this);
        await FirebaseManager.Instance.WaitForInitialization();

        // _roomInfoRepository = new RoomInfoRepository(FirebaseManager.Instance.DB);
        _roomInfoRepository = new RoomInfoRepository(FirebaseFirestore.DefaultInstance);
        AuthenticationManager.Instance.OnLogin += InitializeRoomInfos;
    }
    
    public void SetClientGameMode(string inviteCode) // 임시코드
    {
        InviteCode = inviteCode;
        GameMode = GameMode.Client;
    }

    // 게스트는 이 메서드가 호출되어야 비로소 방 정보를 알게 됩니다.
    public void SetCurrentRoomInfo(RoomInfo roomInfo)
    {
        CurrentRoomInfo = roomInfo;
        Debug.Log($"[RoomInfoManager] CurrentRoomInfo가 설정되었습니다. ID: {CurrentRoomInfo.ID}");
        OnCurrentRoomInfoUpdated?.Invoke();
    }

    public void SetRoomInfoDTO(RoomInfoDTO roomInfoDTO)
    {
        CurrentRoomInfoDTO = roomInfoDTO;
        // CurrentRoomInfo = roomInfoDTO.ToDomain();
        SetCurrentRoomInfo(roomInfoDTO.ToDomain());
        Debug.Log(CurrentRoomInfo.ID);
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
        Debug.Log($"기존 RoomInfo를 수정합니다. ID: {CurrentRoomInfo.ID}");
        await _roomInfoRepository.UpdateRoomInfo(CurrentRoomInfo.ToDTO(), RoomInfoNetworkManager.Instance.UserID);
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

    public async UniTask DeleteRoom(RoomInfoDTO roomInfoDTO)
    {
        Debug.Log(_roomInfoRepository);
        try
        {
            await _roomInfoRepository.DeleteRoomInfo(_userID, roomInfoDTO.RoomInfoID);
            Debug.Log("방이 삭제됩니다.");
        }
        catch (Exception e)
        {
            Debug.LogError("방 삭제 실패");
        }
    }

    public async UniTask<string> GenerateInviteCode()
    {
        // 1. 현재 방 정보와 유저 정보가 유효한지 확인
        if (CurrentRoomInfo == null || string.IsNullOrEmpty(CurrentRoomInfo.ID))
        {
            Debug.LogError("초대 코드를 생성할 현재 방 정보가 없습니다.");
            return null; // 실패 시 null 반환
        }

        // _userID는 AuthenticationManager에서 가져온다고 가정
        if (string.IsNullOrEmpty(_userID))
        {
            Debug.LogError("로그인한 사용자 정보가 없습니다.");
            return null;
        }

        // 2. Repository의 메서드를 호출하여 코드 생성 요청
        try
        {
            var generatedCode = await _roomInfoRepository.CreateInviteCode(_userID, CurrentRoomInfo.ID);
            InviteCode = generatedCode;
            return InviteCode;
        }
        catch (Exception e)
        {
            Debug.LogError($"초대 코드 생성 중 에러 발생: {e.Message}");
            return null;
        }
    }

    // [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    // public void RPC_SyncRoomInfoToNewPlayer(PlayerRef player, string roomInfoJson)
    // {
    //     if (string.IsNullOrEmpty(roomInfoJson))
    //     {
    //         Debug.LogError("수신된 roomInfoJson이 비어있습니다!");
    //         return;
    //     }
    //
    //     Debug.Log($"클라이언트가 호스트로부터 RoomInfo JSON 수신: {roomInfoJson}");
    //
    //     // 1. 먼저 Json을 네트워크 DTO(RoomInfoNetworkDTO)로 변환합니다.
    //     var networkDTO = JsonUtility.FromJson<RoomInfoNetworkDTO>(roomInfoJson);
    //
    //     if (networkDTO == null)
    //     {
    //         Debug.LogError("Json을 RoomInfoNetworkDTO로 변환하는데 실패했습니다.");
    //         return;
    //     }
    //
    //     // 2. 변환된 DTO를 사용하여 최종 RoomInfo 객체를 생성합니다.
    //     // (이전에 RoomInfo 클래스에 만들어 둔 생성자를 활용합니다)
    //     CurrentRoomInfo = new RoomInfo(networkDTO);
    //
    //     Debug.Log($"[RoomInfoManager] 동기화 완료. 방 이름: {CurrentRoomInfo.RoomName}");
    // }
}
