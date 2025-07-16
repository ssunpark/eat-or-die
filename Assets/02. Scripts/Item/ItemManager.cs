using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

// 아이템 생성, 조회, 데이터 로딩
public class ItemManager : NetworkBehaviour
{
    public static ItemManager Instance { get; private set; }

    public override void Spawned()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else
        {
            Runner.Despawn(Object); // 중복 방지
        }
    }
    
    private const string ITEM_CSV_PATH = "/ItemCSV";
    [Header("아이템 오브젝트")]
    [SerializeField]
    private NetworkPrefabRef _itemObjectPrefab;

    // 아이템 종류 별 딕셔너리로 구분됨. (추가 아이템 종류가 생기는 경우 딕셔너리 추가)
    private Dictionary<int, AItem> _itemDict;
    
    // 아이템 팩토리
    private ItemFactory _itemFactory;

    // private void Awake()
    // {
    //     
    // }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _itemFactory = new ItemFactory();
        
        // 데이터 로드 후 생성
        _itemDict = new Dictionary<int, AItem>();
        
        // 사용 아이템
        var useItemRawData = ItemDataLoader.LoadItemRawData<UseItemRawData>($"{Application.streamingAssetsPath}{ITEM_CSV_PATH}/UseItemTestCSV.csv");
        foreach (var data in useItemRawData)
        {
            var useItem = _itemFactory.CreateUseItem(data);
            _itemDict[data.ID] = useItem;
        }
        
        // 장비 아이템
        var equipmentItemRawData = ItemDataLoader.LoadItemRawData<EquipmentItemRawData>($"{Application.streamingAssetsPath}{ITEM_CSV_PATH}/EquipmentItemTestCSV.csv");
        foreach (var data in equipmentItemRawData)
        {
            var useItem = _itemFactory.CreateEquipmentItem(data);
            _itemDict[data.ID] = useItem;
        }
        
        // 무기 아이템
        // var weaponItemRawData = ItemDataLoader.LoadItemRawData<WeaponItemRawData>($"{Application.streamingAssetsPath}{ITEM_CSV_PATH}");
        // foreach (var data in equipmentItemRawData)
        // {
        //     var useItem = _itemFactory.CreateEquipmentItem(data);
        //     _itemDict[data.ID] = useItem;
        // }
    }

    // 아이템 조회 함수 (추가 아이템 종류가 생기는 경우 종류 별 조회 함수 추가)
    // AItem을 동작에 맞는 인터페이스로 변경해서 사용 (InteractionInterface폴더 참고)
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
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_CreateItemObject(int id, int quantity, Vector3 position, Quaternion rotation)
    {
        if (!Runner.IsServer)
        {
            return;
        }
        
        if (!_itemDict.TryGetValue(id, out AItem item))
        {
            throw new Exception("없는 아이템입니다.");
        }
        
        // 네트워크 아이템 오브젝트 생성
        Runner.Spawn(_itemObjectPrefab,
            position: position,
            rotation: rotation,
            inputAuthority: null,
            onBeforeSpawned: (runner, obj) =>
            {
                var item = obj.GetComponent<ItemObject>();
                item.ItemID = id;
                item.Quantity = quantity;
            });
    }
}
