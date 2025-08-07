using System.Collections.Generic;
using UnityEngine;
public class RoomInfoRepository
{
    public static string SaveKey = "RoomTest";
    
    public void Save(RoomInfo roomInfo)
    {
        RoomInfoDTO dto = RoomInfoDTO.FromDomain(roomInfo);
        string data = JsonUtility.ToJson(dto);

        PlayerPrefs.SetString(SaveKey, data);
        PlayerPrefs.Save();
    }

    public bool TryLoad(out RoomInfo roomInfo)
    {
        string jsonData = PlayerPrefs.GetString(SaveKey, null);
        
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