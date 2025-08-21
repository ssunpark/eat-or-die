using Fusion;
using UnityEngine;

// 수현
public abstract class NetworkBehaviourSingleton<T> : NetworkBehaviour
    where T : NetworkBehaviourSingleton<T> // 셀프 타입 제약
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);

                if (_instance == null)
                {
                    Debug.Log($"[NetworkBehaviourSingleton<{typeof(T).Name}>] 인스턴스를 찾을 수 없습니다.");
                }
            }

            return _instance;
        }
    }

    public static bool Exists => _instance != null;

    public override void Spawned()
    {
        base.Spawned();

        if (_instance == null)
        {
            _instance = (T)this;
            return;
        }

        if (_instance != this)
        {
            if (HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}