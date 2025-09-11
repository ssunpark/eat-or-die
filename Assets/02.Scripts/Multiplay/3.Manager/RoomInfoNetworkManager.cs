using Fusion;
using UnityEngine;

public class RoomInfoNetworkManager : NetworkBehaviourSingleton<RoomInfoNetworkManager>
{
    [Networked] [Capacity(64)] public string UserID { get; private set; }
    
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            UserID = AuthenticationManager.Instance.User.UserId;
        }

        RPC_RequestRoomInfoFromHost();
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestRoomInfoFromHost(RpcInfo info = default)
    {
        var roomInfo = RoomInfoManager.Instance.CurrentRoomInfo.ToNetworkDTO();
        var json = JsonUtility.ToJson(roomInfo);
        
        RPC_SyncRoomInfoToNewPlayer(info.Source, json);
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

    public async void LeaveRoom()
    {
        if (Runner != null && Runner.IsRunning)
        {
            Debug.Log($"방에서 나가는 중... IsServer: {Runner.IsServer}, HasStateAuthority: {HasStateAuthority}");
            
            // 호스트인 경우만 모든 클라이언트에게 서버 종료를 알림
            if (Runner.IsServer && HasStateAuthority)
            {
                Debug.Log("호스트가 방을 나갑니다. 모든 클라이언트에게 알림을 보냅니다.");
                RPC_NotifyHostLeaving();
                
                // 잠시 대기 후 서버 종료 (RPC가 전달될 시간을 줌)
                await System.Threading.Tasks.Task.Delay(100);
                await Runner.Shutdown(shutdownReason: Fusion.ShutdownReason.Ok);
            }
            else
            {
                Debug.Log("클라이언트가 방을 나갑니다.");
                await Runner.Shutdown(shutdownReason: Fusion.ShutdownReason.Ok);
            }
        }
        else
        {
            Debug.Log("NetworkRunner가 실행 중이 아닙니다. 로딩 씬으로 이동합니다.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_NotifyHostLeaving()
    {
        Debug.Log("호스트가 방을 나간다는 알림을 받았습니다.");
        
        // 클라이언트인 경우에만 강제 종료
        if (Runner != null && !Runner.IsServer)
        {
            Debug.Log("클라이언트: 호스트가 나갔으므로 정리 후 로딩 씬으로 이동합니다.");
            CleanupAndLoadScene();
        }
    }

    private void CleanupAndLoadScene()
    {
        // DontDestroyOnLoad 객체들 정리
        var room = Room.Instance;
        if (room != null)
        {
            Destroy(room.gameObject);
        }

        // 모든 플레이어 오브젝트 정리
        var players = FindObjectsOfType<Player>();
        foreach (var player in players)
        {
            if (player != null)
            {
                Destroy(player.gameObject);
            }
        }

        // NetworkRunner 종료 후 씬 이동
        if (Runner != null && Runner.IsRunning)
        {
            Runner.Shutdown(shutdownReason: Fusion.ShutdownReason.Ok);
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}