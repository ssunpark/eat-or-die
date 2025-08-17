using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using Fusion;
using UnityEngine;

public class RoomInfoManager : BehaviourSingleton<RoomInfoManager>
{
    public RoomInfo CurrentRoomInfo { get; private set; }
    public RoomInfoDTO CurrentRoomInfoDTO { get; private set; }
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

    public void SetRoomInfoDTO(RoomInfoDTO roomInfoDTO)
    {
        CurrentRoomInfoDTO = roomInfoDTO;
        CurrentRoomInfo = roomInfoDTO.ToDomain();
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
        if (CurrentRoomInfoDTO == null)
        {
            Debug.LogError("저장할 RoomInfo가 없습니다.");
            return;
        }

        // ★★★ 핵심 분기 로직 ★★★
        // CurrentRoomInfo의 ID가 비어있으면 '생성', 아니면 '수정'
        if (string.IsNullOrEmpty(CurrentRoomInfoDTO.RoomInfoID))
        {
            Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        }
        else
        {
            // ID가 있으므로 기존 문서 수정
            Debug.Log($"기존 RoomInfo를 수정합니다. ID: {CurrentRoomInfoDTO.RoomInfoID}");
            await _roomInfoRepository.UpdateRoomInfo(CurrentRoomInfoDTO, _userID);
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SyncRoomInfoToNewPlayer(PlayerRef player, string roomInfoJson)
    {
        if (string.IsNullOrEmpty(roomInfoJson))
        {
            Debug.LogError("수신된 roomInfoJson이 비어있습니다!");
            return;
        }

        Debug.Log($"클라이언트가 호스트로부터 RoomInfo JSON 수신: {roomInfoJson}");

        // 1. 먼저 Json을 네트워크 DTO(RoomInfoNetworkDTO)로 변환합니다.
        var networkDTO = JsonUtility.FromJson<RoomInfoNetworkDTO>(roomInfoJson);

        if (networkDTO == null)
        {
            Debug.LogError("Json을 RoomInfoNetworkDTO로 변환하는데 실패했습니다.");
            return;
        }

        // 2. 변환된 DTO를 사용하여 최종 RoomInfo 객체를 생성합니다.
        // (이전에 RoomInfo 클래스에 만들어 둔 생성자를 활용합니다)
        CurrentRoomInfo = new RoomInfo(networkDTO);

        Debug.Log($"[RoomInfoManager] 동기화 완료. 방 이름: {CurrentRoomInfo.RoomName}");
    }


}
