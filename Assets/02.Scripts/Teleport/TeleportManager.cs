using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TeleportManager : BehaviourSingleton<TeleportManager>
{
    public event Action OnInteractPortal;
    public event Action OnExitPortal;

    public int DepartureStage = -1;
    public int DestinationStage = -1;

    [SerializeField] private CanvasGroup _portalCanvasGroup;

    [SerializeField] private List<GameObject> _destinationList;

    private Player _localPlayer;

    private void Start()
    {
        FadeOutAsync(2f).Forget();
    }

    public void PortalInteract(int stageIndex)
    {
        DepartureStage = stageIndex;
        OnInteractPortal?.Invoke();
    }
    
    public void ClosePortal()
    {
        OnExitPortal?.Invoke();
    }

    public async UniTaskVoid TeleportAsync()
    {
        InputReader.Instance.InputActions.Player.Disable();
        ParticleManager.Instance.PlayByKey(
            "Particle_TeleportPrewarm", 
            Room.Instance.LocalPlayer.transform.position + Vector3.up * 0.1f,
            Quaternion.identity,
            true);

        if(_localPlayer == null)
        {
            _localPlayer = Room.Instance.LocalPlayer.GetComponent<Player>();
            if(_localPlayer == null)
            {
                Debug.LogError("Local player not found.");
                return;
            }
        }

        await UniTask.Delay(TimeSpan.FromSeconds(3));
        ParticleManager.Instance.PlayByKey(
            "Particle_Teleport",
            Room.Instance.LocalPlayer.transform.position + Vector3.up * 0.1f,
            Quaternion.identity,
            true);
        FadeInAsync().Forget();
        _localPlayer.HideCharacter(hide : true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        StageManager.Instance.Transfer(DepartureStage, DestinationStage);
        _localPlayer.Teleport(_destinationList[DestinationStage].transform.position);

        _localPlayer.HideCharacter(hide: false);
        FadeOutAsync(Math.Abs(DestinationStage - DepartureStage) * 1.2f).Forget();

        ParticleManager.Instance.PlayByKey(
            "Particle_Teleport",
            Room.Instance.LocalPlayer.transform.position + Vector3.up * 0.1f,
            Quaternion.identity,
            true);
        DepartureStage = -1;
        DestinationStage = -1;

        InputReader.Instance.InputActions.Player.Enable();
    }

    //페이드 인/아웃
    private async UniTask FadeInAsync()
    {
        _portalCanvasGroup.alpha = 0f;
        _portalCanvasGroup.gameObject.SetActive(true);
        while (_portalCanvasGroup.alpha < 1f)
        {
            _portalCanvasGroup.alpha += Time.deltaTime * 2f;
            await UniTask.Yield();
        }
    }

    private async UniTask FadeOutAsync(float delay = 0f)
    {
        if (delay > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
        }
        while (_portalCanvasGroup.alpha > 0f)
        {
            _portalCanvasGroup.alpha -= Time.deltaTime * 2f;
            await UniTask.Yield();
        }
        _portalCanvasGroup.gameObject.SetActive(false);
    }

    public void Teleport()
    {
        if (DestinationStage == -1)
        {
            UI_Notification.Notify(message: "목적지가 설정되지 않았습니다.");
            return;
        }
        if(DestinationStage > _destinationList.Count - 1)
        {
            UI_Notification.Notify(message: "잘못된 목적지입니다.");
            return;
        }
        if (DepartureStage == DestinationStage)
        {
            // 현재 위치와 같음을 알림
            return;
        }
        if(_localPlayer == null)
        {
            _localPlayer = Room.Instance.LocalPlayer.GetComponent<Player>();
        }
        Debug.Log($"텔레포트: from {DepartureStage}, to {DestinationStage}");
        TeleportAsync().Forget();
    }
}
