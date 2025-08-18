using Fusion;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    
    private Room _currentRoom;

    
    public void OnClickStartHost()
    {
        GenerateRoom();
        //_currentRoom.StartGame(GameMode.Host);
    }
    
    public void OnClickStartClient()
    {
        GenerateRoom();
        //_currentRoom.StartGame(GameMode.Client);
    }

    private void GenerateRoom()
    {
        if (_currentRoom == null)
        {
            
        }
    }
}
