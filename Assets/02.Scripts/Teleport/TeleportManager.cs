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

    [SerializeField] private string _teleportPrewarmKey = "Particle_TeleportPrewarm";
    [SerializeField] private string _teleportKey = "Particle_Teleport";

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

    public async UniTask ReviveTeleport()
    {
        DepartureStage = StageManager.Instance.CurrentStage;
        DestinationStage = 0;

        await TeleportAsync(isRevive: true);
    }

    private void CacheLocal()
    {
        if (_localPlayer == null)
        {
            _localPlayer = Room.Instance.LocalPlayer.GetComponent<Player>();
            if (_localPlayer == null)
            {
                Debug.LogError("Local player not found.");
            }
        }
    }

    /// <summary>
    ///
    /// 텔레포트 기능을 비동기로 처리합니다.
    ///
    /// isRevive가 true일 경우, 텔레포트 위치가 (0, 0.5f, 0)으로 고정됩니다.
    ///
    /// DepartureStage와 DestinationStage를 설정한 후, 텔레포트 이펙트를 재생하고,
    /// 페이드 인/아웃 효과를 적용합니다.
    ///
    /// 로컬 플레이어의 캐릭터를 숨기고, 텔레포트 후 다시 보이게 합니다.
    /// </summary>
    public async UniTask TeleportAsync(bool isRevive = false)
    {
        // 인풋 비활성화
        InputReader.Instance.InputActions.Player.Disable();

        // 혹시 모를 로컬 플레이어 다시 확인
        CacheLocal();

        string additionalKey = isRevive ? "_Revive" : string.Empty;

        // 텔레포트 준비 이펙트
        ParticleManager.Instance.PlayByKey(
            _teleportPrewarmKey + additionalKey,
            _localPlayer.transform.position + Vector3.up * 0.1f,
            Quaternion.identity,
            true);

        MasterAudio.PlaySound3DAtTransform("Teleport", _localPlayer.transform);
        // 3초 대기
        await UniTask.Delay(TimeSpan.FromSeconds(3));
        // 텔레포트 실행
        // 텔레포트 시작 이펙트
        ParticleManager.Instance.PlayByKey(
            _teleportKey + additionalKey,
            _localPlayer.transform.position + Vector3.up * 0.1f,
            Quaternion.identity,
            true);

        // 텔레포트 시작
        FadeInAsync().Forget();
        _localPlayer.HideCharacter(hide: true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        var destination = isRevive ? new Vector3(0, 0.5f, 0) : _destinationList[DestinationStage].transform.position;
        _localPlayer.Teleport(destination);

        float delay = Math.Abs(DestinationStage - DepartureStage) * 1.2f; // 목적지에 따라 딜레이 조정
        FadeOutAsync(delay).Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(delay)); // 페이드 아웃이 시작과 동시에 텔레포트 완료

        // 텔레포트 완료
        _localPlayer.HideCharacter(hide: false);
        _localPlayer.Anim.Play("Teleport_Hard");

        // 베이스캠프에서 죽어서 부활할 때 동일한 스테이지에 대해 Exit, Enter를 동시에 호출해서 발생 할 수 있는 문제를 방지하기 위해 조건 걸어둠
        // 문제 없다면 이 조건은 제거해도 됩니다.
        if (DepartureStage != DestinationStage)
        {
            StageManager.Instance.Transfer(DepartureStage, DestinationStage);
        }

        DepartureStage = -1;
        DestinationStage = -1;

        // 텔레포트 완료 이펙트
        ParticleManager.Instance.PlayByKey(
            _teleportKey + additionalKey,
            _localPlayer.transform.position + Vector3.up * 0.1f,
            Quaternion.identity,
            true);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));

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
        if (DestinationStage > _destinationList.Count - 1)
        {
            UI_Notification.Notify(message: "잘못된 목적지입니다.");
            return;
        }
        if (DepartureStage == DestinationStage)
        {
            UI_Notification.Notify(message: "목적지에 이미 위치하고 있습니다.");
            return;
        }
        Debug.Log($"텔레포트: from {DepartureStage}, to {DestinationStage}");
        MasterAudio.ChangePlaylistByName($"{DestinationStage}FloorBGM");
        TeleportAsync().Forget();
    }
}
