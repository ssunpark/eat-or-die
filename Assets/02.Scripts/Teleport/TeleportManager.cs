using System;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : BehaviourSingleton<TeleportManager>
{
    public event Action OnInteractPortal;

    public int SelectedStage;

    [SerializeField] private List<GameObject> _stageList;

    public void PortalInteract()
    {
        OnInteractPortal?.Invoke();
    }

    public void Teleport()
    {
        if (SelectedStage == 0)
        {
            Debug.Log("목적지가 선택되지 않았습니다.");
            return;
        }
        // FixedUpdateNetwork 타이밍에 순간이동 하도록 처리해야함
        Debug.Log($"텔레포트: {SelectedStage}");
    }
}
