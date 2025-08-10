using System.Collections.Generic;
using UnityEngine;
public class RoomInfoRepository
{
    public void Save(string roomName ,RoomInfoDTO roomInfoDto)
    {
        // firebase Save를 하면 자동 ID가 생성 해당 ID를 고유값으로 사용
        string data = JsonUtility.ToJson(roomInfoDto);

        PlayerPrefs.SetString(roomName, data);
        PlayerPrefs.Save();
    }

    public bool TryLoad(string roomName, out RoomInfo roomInfo)
    {
        string jsonData = PlayerPrefs.GetString(roomName, null);
        
        if (string.IsNullOrEmpty(jsonData))
        {
            roomInfo = new RoomInfo();
            return false; // 저장된 게 없으므로 return false
        }
        
        RoomInfoDTO dto = JsonUtility.FromJson<RoomInfoDTO>(jsonData);
        roomInfo = dto.ToDomain();
        return true; // 저장된 값 정상 로드
    }
}