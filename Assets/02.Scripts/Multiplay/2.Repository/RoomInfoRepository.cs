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