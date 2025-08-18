using System;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : BehaviourSingleton<TeleportManager>
{
    public event Action OnInteractPortal;
    public event Action OnExitPortal;

    public int DepartureStage = -1;
    public int DestinationStage = -1;

    [SerializeField] private List<GameObject> _destinationList;

    public void PortalInteract(int stageIndex)
    {
        DepartureStage = stageIndex;
        OnInteractPortal?.Invoke();
    }
    
    public void ClosePortal()
    {
        OnExitPortal?.Invoke();
    }

    public void Teleport()
    {
        if (DestinationStage == -1)
        {
            Debug.Log("목적지가 선택되지 않았습니다.");
            return;
        }

        if (DepartureStage == DestinationStage)
        {
            // 현재 위치와 같음을 알림
            return;
        }
        
        Debug.Log($"텔레포트: from {DepartureStage}, to {DestinationStage}");
        StageManager.Instance.Transfer(DepartureStage, DestinationStage);
        Room.Instance.LocalPlayer.GetComponent<Player>().Teleport(_destinationList[DestinationStage].transform.position);
        DepartureStage = -1;
        DestinationStage = -1;
    }
}
