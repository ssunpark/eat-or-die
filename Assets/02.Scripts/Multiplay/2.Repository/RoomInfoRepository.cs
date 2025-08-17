using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

public class RoomInfoRepository
{
    private readonly FirebaseFirestore _db;

    // 의존성 주입
    public RoomInfoRepository(FirebaseFirestore db)
    {
        _db = db;
    }

    // 유저 컬렉션 안의 방 목록 가져오기
    public async UniTask<List<RoomInfoDTO>> GetRoomInfosByUserId(string userId)
    {
        var roomInfoDtos = new List<RoomInfoDTO>();
        try
        {
            var query = _db.Collection("Users")
                .Document(userId)
                .Collection("Rooms")
                .OrderByDescending("RoomName");

            var snapshot = await query.GetSnapshotAsync();
            foreach (var doc in snapshot.Documents)
            {
                roomInfoDtos.Add(doc.ConvertTo<RoomInfoDTO>());
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching room infos for user {userId}: {e.Message}");
        }

        return roomInfoDtos;
    }

    public async UniTask<RoomInfoDTO> GetRoomInfoById(string userId, string roomId)
    {
        try
        {
            var docRef = _db.Collection("Users")
                .Document(userId)
                .Collection("Rooms")
                .Document(roomId);

            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<RoomInfoDTO>();
            }

            Debug.LogWarning($"RoomInfo {roomId} not found for user {userId}");
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"RoomInfo 가져오기 실패: {e.Message}");
            throw;
        }
    }
    

    // 방 추가
    public async UniTask AddRoomInfo(RoomInfoDTO roomInfoDTO, string userId)
    {
        try
        {
            var docRef = _db.Collection("Users")
                .Document(userId)
                .Collection("Rooms")
                .Document(); // 자동 ID 생성

            await docRef.SetAsync(roomInfoDTO);
        }
        catch (Exception e)
        {
            throw new Exception($"RoomInfo 추가 실패: {e.Message}");
            throw;
        }
    }

    // 방 업데이트
    public async UniTask UpdateRoomInfo(RoomInfoDTO roomInfoDto, string userId)
    {
        // ★★★ 이 디버그 코드를 추가해서 실제 값을 확인하세요! ★★★
        Debug.Log($"[UpdateRoomInfo] 업데이트 시도: UserID='{userId}', RoomInfoID='{roomInfoDto?.RoomInfoID}'");

        // roomInfoDto 자체가 null인지도 확인
        if (roomInfoDto == null)
        {
            Debug.LogError("[UpdateRoomInfo] roomInfoDto 객체 자체가 null입니다!");
            return;
        }

        // ID들이 비어있는지 명시적으로 확인
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roomInfoDto.RoomInfoID))
        {
            Debug.LogError("[UpdateRoomInfo] UserID 또는 RoomInfoID가 null이거나 비어있습니다. 업데이트를 중단합니다.");
            return;
        }
        
        try
        {
            var docRef = _db.Collection("Users")
                .Document(userId)
                .Collection("Rooms")
                .Document(roomInfoDto.RoomInfoID);

            var updates = new Dictionary<string, object>
            {
                { "RoomName", roomInfoDto.RoomName },
                { "KnownIngredientsList", roomInfoDto.KnownIngredientsList },
                { "KnownRecipesList", roomInfoDto.KnownRecipesList }
            };

            await docRef.UpdateAsync(updates);
        }
        catch (Exception e)
        {
            Debug.LogError($"RoomInfo 갱신 실패: {e.Message}");
            throw;
        }
    }

    // 방 삭제
    public async UniTask<bool> DeleteRoomInfo(string userId, string roomId)
    {
        try
        {
            var docRef = _db.Collection("Users")
                .Document(userId)
                .Collection("Rooms")
                .Document(roomId);

            await docRef.DeleteAsync();
            Debug.Log($"RoomInfo {roomId} 삭제 완료");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"RoomInfo 삭제 실패: {e.Message}");
            return false;
        }
    }

    public async UniTask<string> CreateInviteCode(string inviterId, string roomInfoId)
    {
        string code = InviteCodeGenerator.GenerateCode();

        var docRef = _db.Collection("inviteCodes").Document(code);

        // 저장할 데이터
        var data = new
        {
            inviter = inviterId,
            roomInfoId,
            used = false,
            usedBy = (string)null,
            createdAt = Timestamp.GetCurrentTimestamp()
        };

        await docRef.SetAsync(data);

        Debug.Log("Created Invite Code: " + code);
        return code;
    }
// 안쓸 예정
    // public void Save(string roomName ,RoomInfoDTO roomInfoDto)
    // {
    //     // firebase Save를 하면 자동 ID가 생성 해당 ID를 고유값으로 사용
    //     string data = JsonUtility.ToJson(roomInfoDto);
    //
    //     PlayerPrefs.SetString(roomName, data);
    //     PlayerPrefs.Save();
    // }

    // public bool TryLoad(string roomName, out RoomInfo roomInfo)
    // {
    //     string jsonData = PlayerPrefs.GetString(roomName, null);
    //     
    //     if (string.IsNullOrEmpty(jsonData))
    //     {
    //         roomInfo = new RoomInfo();
    //         return false; // 저장된 게 없으므로 return false
    //     }
    //     
    //     RoomInfoDTO dto = JsonUtility.FromJson<RoomInfoDTO>(jsonData);
    //     roomInfo = dto.ToDomain();
    //     return true; // 저장된 값 정상 로드
    // }
}