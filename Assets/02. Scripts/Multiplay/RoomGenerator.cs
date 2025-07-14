using Fusion;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _roomPrefab;
    private Room _currentRoom;

    public void OnClickStartHost()
    {
        GenerateRoom();
        _currentRoom.StartGame(GameMode.Host);
    }
    
    public void OnClickStartClient()
    {
        GenerateRoom();
        _currentRoom.StartGame(GameMode.Client);
    }

    private void GenerateRoom()
    {
        if (_currentRoom == null)
        {
            _currentRoom = Instantiate(_roomPrefab).GetComponent<Room>();
        }
    }
}
