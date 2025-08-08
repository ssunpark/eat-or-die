using Fusion;
using UnityEngine;
// 수현
public interface IRoomState
{
    public void OnRegister(); // 동적으로 등록
    public void OnUnregister(); // 등록 해제
    public void OnPlayerJoined(PlayerRef player);
    public void OnPlayerLeft(PlayerRef player);
}
