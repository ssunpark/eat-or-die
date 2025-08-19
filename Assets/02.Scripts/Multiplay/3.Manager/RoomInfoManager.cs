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
    public string InviteCode;
    public GameMode GameMode; // 임시 코드

    public async void Awake()
    {
        DontDestroyOnLoad(this);
        await FirebaseManager.Instance.WaitForInitialization();
        
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
        if (CurrentRoomInfo == null || string.IsNullOrEmpty(CurrentRoomInfo.ID))
        {
            Debug.LogError("초대 코드를 생성할 현재 방 정보가 없습니다.");
            return null;
        }
        
        if (string.IsNullOrEmpty(_userID))
        {
            Debug.LogError("로그인한 사용자 정보가 없습니다.");
            return null;
        }
        
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
}
