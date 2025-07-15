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
    // Addressable Path
    public readonly string IconAddressablePath;
    private Sprite _icon;
    public Sprite Icon => _icon;
    // .. 등등 추가 예정

    public ItemData(int id, string name, string description, int maxQuantity, string iconAddressablePath)
    {
        // TODO: 유효성 검사
        ID = id;
        Name = name;
        Description = description;
        MaxQuantity = maxQuantity;
        IconAddressablePath = iconAddressablePath;

        Addressables.LoadAssetAsync<Sprite>("TestItemIcon").Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _icon = handle.Result;
            }
            else
            {
                throw new Exception("아이콘 로드에 실패했습니다.");
            }
        };
    }
}