using System;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : BehaviourSingleton<TeleportManager>
{
    public event Action OnInteractPortal;

    public int DepartureStage;
    public int DestinationStage;

    [SerializeField] private List<GameObject> _teleportPoints;

    public void PortalInteract(int stageIndex)
    {
        DepartureStage = stageIndex;
        OnInteractPortal?.Invoke();
    }
    
    public void ClosePortal()
    {
        OnInteractPortal?.Invoke();
    }

    public void Teleport()
    {
        if (DestinationStage == 0)
        {
            Debug.Log("목적지가 선택되지 않았습니다.");
            return;
        }
        // FixedUpdateNetwork 타이밍에 순간이동 하도록 처리해야함
        Debug.Log($"텔레포트: from {DepartureStage}, to {DestinationStage}");

        // 플레이어 이동 로직 실행
    }
}
