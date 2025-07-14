using System;
using System.Collections.Generic;
using UnityEngine;

// 아이템 생성, 조회, 데이터 로딩
public class ItemManager : MonoBehaviour
{
    private const string DEFAULT_CSV_PATH = "/10. CSV";

    // 아이템 종류 별 딕셔너리로 구분됨. (추가 아이템 종류가 생기는 경우 딕셔너리 추가)
    private Dictionary<int, AItem> _itemDict;
    
    // 아이템 팩토리
    private ItemFactory _itemFactory;

    private void Awake()
    {
        _itemFactory = new ItemFactory();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        // 데이터 로드 후 생성
        _itemDict = new Dictionary<int, AItem>();
        var useItemRawData = ItemDataLoader.LoadUseItemRawData($"{Application.dataPath}{DEFAULT_CSV_PATH}/UseItemTestCSV.csv");
        foreach (var data in useItemRawData)
        {
            var useItem = _itemFactory.CreateUseItem(data);
            _itemDict[data.ID] = useItem;
        }
    }

    // 아이템 조회 함수 (추가 아이템 종류가 생기는 경우 종류 별 조회 함수 추가)
    public AItem GetItem(int id)
    {
        if (_itemDict.TryGetValue(id, out AItem item))
        {
            return item;
        }
        throw new Exception("존재하지 않는 사용 아이템입니다.");
        return null;
    }

    /// <summary>
    /// 아이템 생성(드랍)
    /// </summary>
    /// <param name="id">아이템 ID</param>
    /// <param name="quantity">수량</param>
    public void CreateItemObject(int id, int quantity)
    {
        if (!_itemDict.TryGetValue(id, out AItem item))
        {
            throw new Exception("없는 아이템입니다.");
        }
        
        ItemStack itemStack = new ItemStack(id, item.ItemData.MaxQuantity, quantity);
        // 네트워크 아이템 오브젝트 생성
    }
}
