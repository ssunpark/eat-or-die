using Fusion;
using UnityEngine;

public class RoomInfoNetworkManager : NetworkBehaviourSingleton<RoomInfoNetworkManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SyncRoomInfoToNewPlayer([RpcTarget] PlayerRef player, string roomInfoJson)
    {
        if (string.IsNullOrEmpty(roomInfoJson))
        {
            Debug.LogError("수신된 roomInfoJson이 비어있습니다!");
            return;
        }

        Debug.Log($"클라이언트가 호스트로부터 RoomInfo JSON 수신: {roomInfoJson}");

        // 1. 먼저 Json을 네트워크 DTO(RoomInfoNetworkDTO)로 변환합니다.
        var networkDTO = JsonUtility.FromJson<RoomInfoNetworkDTO>(roomInfoJson);

        if (networkDTO == null)
        {
            Debug.LogError("Json을 RoomInfoNetworkDTO로 변환하는데 실패했습니다.");
            return;
        }

        // 2. 변환된 DTO를 사용하여 최종 RoomInfo 객체를 생성합니다.
        // (이전에 RoomInfo 클래스에 만들어 둔 생성자를 활용합니다)
        var newRoomInfo = new RoomInfo(networkDTO);
        RoomInfoManager.Instance.SetCurrentRoomInfo(newRoomInfo);
        Debug.Log($"[RoomInfoManager] 동기화 완료. 방 이름: {newRoomInfo.RoomName}");
    }
}