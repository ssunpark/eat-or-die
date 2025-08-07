using System.Collections.Generic;
using UnityEngine;
public class RoomInfoRepository
{
    public static string SaveKey = "RoomInfo";
    
    public void Save(RoomInfo roomInfo)
    {
        RoomInfoDTO dto = RoomInfoDTO.FromDomain(roomInfo);
        string data = JsonUtility.ToJson(dto);

        PlayerPrefs.SetString(SaveKey, data);
        PlayerPrefs.Save();
    }

    public RoomInfo Load()
    {
        string jsonData = PlayerPrefs.GetString(SaveKey, null);
        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.Log("");
            return new RoomInfo();
        }
        RoomInfoDTO dto = JsonUtility.FromJson<RoomInfoDTO>(jsonData);
        return dto.ToDomain();
    }
}