using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DarkTonic.MasterAudio;
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
        // 인풋 비활성화
        InputReader.Instance.InputActions.Player.Disable();

        // 텔레포트 준비 이펙트
        ParticleManager.Instance.PlayByKey(
            "Particle_TeleportPrewarm", 
            Room.Instance.LocalPlayer.transform.position + Vector3.up * 0.1f,
            Quaternion.identity,
            true);

        // 혹시 모를 로컬 플레이어 다시 확인
        if(_localPlayer == null)
        {
            _localPlayer = Room.Instance.LocalPlayer.GetComponent<Player>();
            if(_localPlayer == null)
            {
                Debug.LogError("Local player not found.");
                return;
            }
        }
        
        // 텔레포트 사운드
        MasterAudio.PlaySound3DAtTransform("Teleport", _localPlayer.transform);

        // 3초 대기
        await UniTask.Delay(TimeSpan.FromSeconds(3));

        // 텔레포트 시작 이펙트
        ParticleManager.Instance.PlayByKey(
            "Particle_Teleport",
            Room.Instance.LocalPlayer.transform.position + Vector3.up * 0.1f,
            Quaternion.identity,
            true);


        // 텔레포트 시작
        FadeInAsync().Forget();
        _localPlayer.HideCharacter(hide : true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        _localPlayer.Teleport(_destinationList[DestinationStage].transform.position);

        float delay = Math.Abs(DestinationStage - DepartureStage) * 1.2f; // 목적지에 따라 딜레이 조정
        FadeOutAsync(delay).Forget();
        
        await UniTask.Delay(TimeSpan.FromSeconds(delay)); // 페이드 아웃이 시작과 동시에 텔레포트 완료

        // 텔레포트 완료
        _localPlayer.HideCharacter(hide: false);
        StageManager.Instance.Transfer(DepartureStage, DestinationStage);
        DepartureStage = -1;
        DestinationStage = -1;

        // 텔레포트 완료 이펙트
        ParticleManager.Instance.PlayByKey(
            "Particle_Teleport",
            Room.Instance.LocalPlayer.transform.position + Vector3.up * 0.1f,
            Quaternion.identity,
            true);

        // 인풋 활성화
        InputReader.Instance.InputActions.Player.Enable();
    }

    //페이드 인
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

    /// <summary>
    /// delay 후 페이드 아웃
    /// </summary>
    /// <param name="delay">기다릴 시간</param>
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
            UI_Notification.Notify(message: "목적지에 이미 위치하고 있습니다.");
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
