using UnityEngine;
public class RoomInfoRepository
{
    private RoomInfo _roomInfo;

    public static string SaveKey = "RoomInfo";

    // 방장 기준으로 저장 (추후 데베)
    public void Save(RoomInfo roomInfo)
    {
        string data = JsonUtility.ToJson(roomInfo);

        PlayerPrefs.SetString(SaveKey, data);
        PlayerPrefs.Save();
    }

    public RoomInfo Load(RoomInfo roomInfo)
    {
        string jsonData = PlayerPrefs.GetString(SaveKey, null);
        _roomInfo = JsonUtility.FromJson<RoomInfo>(jsonData);

        return _roomInfo;
    }
}