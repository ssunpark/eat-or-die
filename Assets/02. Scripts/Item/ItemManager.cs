using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private const string DEFAULT_CSV_PATH = "/10. CSV";
    // 아이템 생성, 조회, 데이터 로딩

    // 아이템 종류 별 딕셔너리로 구분됨. (추가 아이템 종류가 생기는 경우 딕셔너리 추가)
    private Dictionary<string, UseItem> _useItemDict;
    
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
        _useItemDict = new Dictionary<string, UseItem>();
        var useItemRawData = ItemDataLoader.LoadUseItemRawData($"{Application.dataPath}{DEFAULT_CSV_PATH}/UseItemTestCSV.csv");
        foreach (var data in useItemRawData)
        {
            var useItem = _itemFactory.CreateUseItem(data);
            _useItemDict[data.ID] = useItem;
        }
    }

    // 아이템 조회 함수 (추가 아이템 종류가 생기는 경우 종류 별 조회 함수 추가)
    public UseItem GetUseItem(string key)
    {
        if (_useItemDict.TryGetValue(key, out UseItem item))
        {
            return item;
        }
        throw new Exception("존재하지 않는 사용 아이템입니다.");
        return null;
    }
}
