using System.Threading;
using Cysharp.Threading.Tasks;
using DarkTonic.MasterAudio;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadingSceneOrchestrator : MonoBehaviour
{
    [Header("Runner")]
    [SerializeField] private GameObject _roomPrefab;

    [Header("Scenes")]
    [SerializeField] private string _gameSceneName = "MapScene_UI"; // Scene3 이름

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Slider _progressSlider;


    [SerializeField] private NetworkPrefabRef _playerInfoManagerRef;

    private const float _preloadWeight = 0.8f; // 프리로드 80%
    private const float _connectWeight = 0.2f; // 연결/전환 20%
    private NetworkRunner _runner;
    private float _targetProgress; // 부드러운 슬라이더용

    private void Start()
    {
        _targetProgress = 0f;
        if (_progressSlider) _progressSlider.value = 0f;
        if (_statusText) _statusText.text = "초기화 중...";
        StartFlowAsync().Forget();
    }

    private void Update()
    {
        // 슬라이더 부드럽게
        if (_progressSlider)
            _progressSlider.value = Mathf.MoveTowards(_progressSlider.value, _targetProgress, Time.deltaTime * 0.75f);
    }

    private async UniTaskVoid StartFlowAsync()
    {
        var token = this.GetCancellationTokenOnDestroy();

        // 1) (로컬) 프리로드
        await PreloadAsync(token);

        // 2) Runner 생성/시작 (붙이기)
        await CreateAndStartRunnerAsync(token);
    }

    private async UniTask PreloadAsync(CancellationToken token)
    {
        SetStatus("리소스 준비 중...");
        SetProgressImmediate(0f);

        //if (ParticleManager.Instance != null)
        //{
        //    await ParticleManager.Instance.InitFromCsvAsync().AttachExternalCancellation(token);
        //    // 40% 근처
        //    SetProgressWeighted(0.4f);
        //}

        // TODO: Addressables/사운드/셰이더 Warmup 등 단계별로 나누면 아래처럼 누적
        // await SomethingAsync(); SetProgressWeighted(0.6f);
        // await SomethingElseAsync(); SetProgressWeighted(0.75f);

        // 프리로드 마무리
        var masterAudio = FindAnyObjectByType<MasterAudio>().gameObject;
        Destroy(masterAudio);
        var playlist = FindAnyObjectByType<PlaylistController>().gameObject;
        Destroy(playlist);
        SetProgressWeighted(0.8f);
        SetStatus("프리로드 완료");
        await UniTask.Yield(token);
    }

    private async UniTask CreateAndStartRunnerAsync(CancellationToken token)
    {
        SetStatus("네트워크 연결 준비 중...");

        if (_runner == null)
        {
            // Room 등 ICallbacks 구현체 연결(있으면)
            var room = Instantiate(_roomPrefab).GetComponent<Room>();
            if (room != null)
            {
                _runner = room.gameObject.AddComponent<NetworkRunner>();

                DontDestroyOnLoad(_runner.gameObject);
                _runner.ProvideInput = true;
                _runner.AddCallbacks(room);
                room.SetRunner(_runner);
            }

            var sceneMgr = _runner.GetComponent<NetworkSceneManagerDefault>()
                          ?? _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

            var args = new StartGameArgs
            {
                GameMode = RoomInfoManager.Instance?.GameMode??GameMode.AutoHostOrClient,
                SessionName = string.IsNullOrEmpty(RoomInfoManager.Instance?.InviteCode)
                              ? "WHYITISFUCKINGNULLOREMPTY"
                              : RoomInfoManager.Instance.InviteCode,
                SceneManager = sceneMgr
            };

            SetStatus("세션 연결 중...");
            _ = SmoothProgressTo(0.9f, 1.25f, token);

            // ★ 먼저 StartGame
            await _runner.StartGame(args);
        }

        // ★ 서버(호스트)일 때만 글로벌 매니저 스폰 → 그리고 씬 전환
        if (_runner.IsServer || _runner.GameMode == GameMode.Host)
        {
            // PlayerInfoManager 네트워크 싱글톤 동적 스폰 (중복 가드)
            if (PlayerInfoManager.Instance == null)
            {
                var pimObj = _runner.Spawn(_playerInfoManagerRef, Vector3.zero, Quaternion.identity);
                // 서버 쪽에서만 Runner 주입(클라는 Room.OnSceneLoadDone에서 주입)
                var pim = pimObj.GetComponent<PlayerInfoManager>();
                if (pim != null) _runner.AddCallbacks(pim);
            }

            SetStatus("게임 씬 전환 중...");
            _ = SmoothProgressTo(0.95f, 0.75f, token);

            await _runner.LoadScene(SceneRef.FromIndex(3));


            // 안전 대기: 활성 씬 이름 확인
            while (SceneManager.GetActiveScene().name != _gameSceneName)
                await UniTask.Yield(token);

            SetStatus("완료");
            SetProgressImmediate(1f);
            return;
        }

        // 클라이언트: 호스트 활성 씬으로 자동 싱크
        SetStatus("호스트 씬 동기화 대기...");
        while (SceneManager.GetActiveScene().name != _gameSceneName)
        {
            var t = 0.90f + 0.08f * Mathf.PingPong(Time.time * 0.5f, 1f);
            SetProgressImmediate(t);
            await UniTask.Yield(token);
        }

        SetStatus("완료");
        SetProgressImmediate(1f);
    }



    #region UI Helpers
    private void SetStatus(string msg)
    {
        if (_statusText) _statusText.text = msg;
    }

    // 프리로드 구간(0~0.8)에서의 누적 반영
    private void SetProgressWeighted(float preload01)
    {
        preload01 = Mathf.Clamp01(preload01);
        _targetProgress = Mathf.Clamp01(preload01 * _preloadWeight);
    }

    // 즉시 지정(연결/대기 구간에도 사용)
    private void SetProgressImmediate(float v01)
    {
        _targetProgress = Mathf.Clamp01(v01);
        if (_progressSlider) _progressSlider.value = _targetProgress;
    }

    // 일정 시간 동안 목표치까지 스무스하게 올림
    private async UniTask SmoothProgressTo(float target01, float duration, CancellationToken token)
    {
        target01 = Mathf.Clamp01(target01);
        duration = Mathf.Max(0.01f, duration);
        float start = _targetProgress;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            _targetProgress = Mathf.Lerp(start, target01, t / duration);
            await UniTask.Yield(token);
        }
        _targetProgress = target01;
    }
    #endregion
}
