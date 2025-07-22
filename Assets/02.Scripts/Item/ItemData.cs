using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemData
{
    // 아이템에 공통적인 데이터 담는 클래스
    public readonly int ID;
    public readonly string Name;
    public readonly string Description;
    public readonly int MaxQuantity;
    public readonly bool Cookable;
    private Sprite _icon;
    public Sprite Icon => _icon;
    // .. 등등 추가 예정

    public ItemData(int id, string name, string description, bool cookable, int maxQuantity, string iconAddressablePath)
    {
        // TODO: 유효성 검사
        ID = id;
        Name = name;
        Description = description;
        Cookable = cookable;
        MaxQuantity = maxQuantity;
        var finalIconAddressablePath = iconAddressablePath == String.Empty ? "TestItemIcon" : iconAddressablePath;

        Addressables.LoadAssetAsync<Sprite>(finalIconAddressablePath).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _icon = handle.Result;
            }
            else
            {
                throw new Exception($"아이콘 로드에 실패했습니다. 아이콘 경로: {finalIconAddressablePath}");
            }
        };
    }
}