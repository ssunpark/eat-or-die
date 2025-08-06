using Fusion;
using UnityEngine;
// 수현
public abstract class NetworkBehaviourSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    private static T i = null;

    public static T Instance
    {
        get
        {
            if (i == null)
            {
                i = FindFirstObjectByType<T>();
                if (i == null)
                {
                    Debug.Log($"[NetworkBehaviourSingleton<{{typeof(T).Name}}>] 인스턴스를 찾을 수 없습니다.");
                }
            }
            return i;
        }
    }

    public override void Spawned()
    {
        base.Spawned();
        if (i == null)
        {
            i = this as T;
        }
        else if (i != this)
        {
            Debug.Log($"[NetworkBehaviourSingleton<{typeof(T).Name}>] 이미 인스턴스가 존재합니다. 중복 생성됨: {name}");
        }
    }

    protected virtual void OnDestroy()
    {
        if (i == this)
        {
            i = null;
        }
    }
}
