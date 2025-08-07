using System.Collections.Generic;
using UnityEngine;
public class RoomInfoRepository
{
    private RoomInfo _roomInfo;

    public static string SaveKey = "RoomInfo";
    
    public void Save(RoomInfo roomInfo)
    {
        RoomInfoDTO dto = RoomInfoDTO.FromDomain(roomInfo);
        string data = JsonUtility.ToJson(dto);

        PlayerPrefs.SetString(SaveKey, data);
        PlayerPrefs.Save();
    }

    public RoomInfo Load(RoomInfo roomInfo)
    {
        string jsonData = PlayerPrefs.GetString(SaveKey, null);
        if (string.IsNullOrEmpty(jsonData))
        {
            return new RoomInfo(new HashSet<int>());
        }
        RoomInfoDTO dto = JsonUtility.FromJson<RoomInfoDTO>(jsonData);
        return dto.ToDomain();
    }
}