using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Debug = UnityEngine.Debug;

public class FirebaseManager : BehaviourSingleton<FirebaseManager>
{
    private FirebaseApp _app;
    public FirebaseApp App => _app;
    
    private FirebaseAuth _auth;
    public FirebaseAuth Auth => _auth;

    private FirebaseFirestore _db;
    public FirebaseFirestore DB => _db;
    
    // Game을 실행하고 로딩할 때 확인 할 변수. Instance가 null이 아니고 IsInitialized가 true면 Firebase가 초기화된 상태.
    public bool IsInitialized => _app != null && _auth != null && _db != null;

    private readonly UniTaskCompletionSource _initTcs = new UniTaskCompletionSource();
    
    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);

        try
        {
            await InitAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async UniTask InitAsync()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyStatus == DependencyStatus.Available)
        {
            var defaultApp = FirebaseApp.DefaultInstance;

            int pid = Process.GetCurrentProcess().Id;
            string appName = $"client-{pid}";
            
            _app = FirebaseApp.GetInstance(appName) ?? FirebaseApp.Create(defaultApp.Options, appName);
            _auth = FirebaseAuth.GetAuth(_app);
            _db = FirebaseFirestore.GetInstance(_app);

            _initTcs.TrySetResult(); // 초기화 완료 알
        }
        else
        {
            Debug.LogError($"파이어베이스 연결에 실패했습니다. {dependencyStatus}");
            _initTcs.TrySetException(new Exception("Firebase initialization failed"));
        }
    }

    public async UniTask WaitForInitialization()
    {
        if (IsInitialized)
        {
            return;
        }

        await _initTcs.Task;
    }
}