using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class TraitIconLoader
{
    private static readonly Dictionary<string, Sprite> _cache = new();
    private const string _fallbackIconPath = "FallbackIcon";

    public static void LoadIcon(string path, Action<Sprite> onLoaded)
    {
        if (string.IsNullOrEmpty(path))
        {
            LoadFallbackIcon(onLoaded);
            return;
        }

        if (_cache.TryGetValue(path, out var cached))
        {
            onLoaded?.Invoke(cached);
            return;
        }

        Addressables.LoadAssetAsync<Sprite>(path).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _cache[path] = handle.Result;
                onLoaded?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogWarning($"[TraitIconLoader] Failed to load icon: {path}, fallback to {_fallbackIconPath}");
                LoadFallbackIcon(onLoaded);
            }
        };
    }

    private static void LoadFallbackIcon(Action<Sprite> onLoaded)
    {
        if (_cache.TryGetValue(_fallbackIconPath, out var fallback))
        {
            onLoaded?.Invoke(fallback);
            return;
        }

        Addressables.LoadAssetAsync<Sprite>(_fallbackIconPath).Completed += fallbackHandle =>
        {
            if (fallbackHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _cache[_fallbackIconPath] = fallbackHandle.Result;
                onLoaded?.Invoke(fallbackHandle.Result);
            }
            else
            {
                Debug.LogError($"[TraitIconLoader] Failed to load fallback icon: {_fallbackIconPath}");
                onLoaded?.Invoke(null);
            }
        };
    }

    /// <summary>
    /// 모든 특성 아이콘을 로드하며, 각 아이콘이 로드될 때마다 콜백을 호출하고 완료 시점에도 콜백을 실행합니다.
    /// </summary>
    public static void LoadAllIcons(IEnumerable<CharacterTraitData> traitDataList,
                                    Action<ETraitType, Sprite> onEachLoaded,
                                    Action onAllCompleted)
    {
        int pendingCount = 0;

        foreach (var data in traitDataList)
        {
            pendingCount++;
            LoadIcon(data.IconPath, sprite =>
            {
                onEachLoaded?.Invoke(data.TraitType, sprite);
                pendingCount--;

                if (pendingCount == 0)
                    onAllCompleted?.Invoke();
            });
        }

        if (pendingCount == 0)
        {
            onAllCompleted?.Invoke();
        }
    }
}
