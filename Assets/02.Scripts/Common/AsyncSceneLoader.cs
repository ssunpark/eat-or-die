using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine.UI;

public class AsyncSceneLoader : MonoBehaviour
{
    [SerializeField] private string _sceneName = "MapScene_UI"; // 로드할 씬 
    [SerializeField] private TextMeshProUGUI _loadingText; // 로딩 텍스트 UI
    [SerializeField] private Slider _loadingSlider; // 로딩 슬라이더 UI
    private const float _particleWeight = 0.2f; // 20%
    private const float _sceneWeight = 0.8f; // 80%

    private void Start()
    {
        StartAsync().Forget();
    }

    private async UniTaskVoid StartAsync()
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        if (_loadingText != null) _loadingText.text = "파티클 로딩...";
        if (_loadingSlider != null) _loadingSlider.value = 0f;

        await ParticleManager.Instance.InitFromCsvAsync().AttachExternalCancellation(token);
        if (_loadingSlider != null) _loadingSlider.value = _particleWeight;

        if (_loadingText != null) _loadingText.text = "씬 로딩 중...";
        var asyncOp = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Single);
        asyncOp.allowSceneActivation = false;

        while (asyncOp.progress < 0.9f)
        {
            float norm = Mathf.InverseLerp(0f, 0.9f, asyncOp.progress);
            float weighted = _particleWeight + (norm * _sceneWeight);
            if (_loadingSlider != null) _loadingSlider.value = weighted;

            await UniTask.Yield(token);
        }

        if (_loadingSlider != null) _loadingSlider.value = 1f;
        asyncOp.allowSceneActivation = true;
    }
}
