//using UnityEngine;
//using UnityEngine.SceneManagement;
//using Cysharp.Threading.Tasks;
//using System.Threading;
//using TMPro;
//using UnityEngine.UI;
//using Fusion; // ★ 추가

//public class AsyncSceneLoader : MonoBehaviour
//{
//    [SerializeField] private string _sceneName = "MapScene_UI"; // 게임 씬 이름 (Scene3)
//    [SerializeField] private TextMeshProUGUI _loadingText;
//    [SerializeField] private Slider _loadingSlider;

//    private const float _particleWeight = 0.2f; // 20%
//    private const float _sceneWeight = 0.8f;    // 80%

//    private void Start()
//    {
//        StartAsync().Forget();
//    }

//    private async UniTaskVoid StartAsync()
//    {
//        var token = this.GetCancellationTokenOnDestroy();

//        if (_loadingText != null) _loadingText.text = "파티클 로딩...";
//        if (_loadingSlider != null) _loadingSlider.value = 0f;

//        // 1) 파티클/프리로드
//        await ParticleManager.Instance.InitFromCsvAsync().AttachExternalCancellation(token);
//        if (_loadingSlider != null) _loadingSlider.value = _particleWeight;

//        // 2) Fusion Runner 유무 체크
//        var runner = FindFirstObjectByType<NetworkRunner>();

//        // 2-1) 호스트라면 Fusion이 씬 전환을 수행 (동기화됨)
//        if (runner != null && (runner.IsServer || runner.GameMode == GameMode.Host))
//        {
            
//            if (_loadingText != null) _loadingText.text = "게임 씬 전환 중...";

//            // Fusion 2 API: 프로젝트 버전에 따라 SetActiveScene 또는 LoadScene 사용
//            await runner.LoadScene(SceneRef.FromIndex(3));
//            // 위 await가 끝나면 보통 이미 전환 완료 상태지만,
//            // 안전하게 ActiveScene이 바뀔 때까지 한 틱 더 대기
//            while (SceneManager.GetActiveScene().name != _sceneName)
//            {
//                if (_loadingSlider != null)
//                    _loadingSlider.value = Mathf.MoveTowards(_loadingSlider.value, 1f, Time.deltaTime * 0.5f);
//                await UniTask.Yield(token);
//            }

//            if (_loadingSlider != null) _loadingSlider.value = 1f;
//            return; // 끝
//        }

//        // 2-2) 클라이언트라면 호스트가 씬을 전환할 때까지 대기 (UI만 업데이트)
//        if (runner != null && !runner.IsServer)
//        {
//            if (_loadingText != null) _loadingText.text = "호스트 전환 대기 중...";

//            // 로딩 바는 80~100% 사이에서 살짝 흔들리며 대기
//            while (SceneManager.GetActiveScene().name != _sceneName)
//            {
//                if (_loadingSlider != null)
//                    _loadingSlider.value = 0.8f + 0.2f * Mathf.PingPong(Time.time * 0.5f, 1f);
//                await UniTask.Yield(token);
//            }

//            if (_loadingSlider != null) _loadingSlider.value = 1f;
//            return; // 끝
//        }

//        // 2-3) Runner가 없으면(에디터/로컬) 기존 Unity 로딩으로 폴백
//        if (_loadingText != null) _loadingText.text = "씬 로딩 중...";
//        var asyncOp = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Single);
//        asyncOp.allowSceneActivation = false;

//        while (asyncOp.progress < 0.9f)
//        {
//            float norm = Mathf.InverseLerp(0f, 0.9f, asyncOp.progress);
//            float weighted = _particleWeight + (norm * _sceneWeight);
//            if (_loadingSlider != null) _loadingSlider.value = weighted;

//            await UniTask.Yield(token);
//        }

//        if (_loadingSlider != null) _loadingSlider.value = 1f;
//        asyncOp.allowSceneActivation = true;
//    }
//}
