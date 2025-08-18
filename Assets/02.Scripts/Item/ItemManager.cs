using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

// 아이템 생성, 조회, 데이터 로딩
public class ItemManager : NetworkBehaviour
{
    public static ItemManager Instance { get; private set; }
    
    private const string FOOD_CSV_PATH = "/ItemCSV/Food.csv";
    private const string TOOL_CSV_PATH = "/ItemCSV/Tool.csv";
    private const string SEED_CSV_PATH = "/ItemCSV/Seed.csv";
    private const string WEAPON_CSV_PATH = "/ItemCSV/Weapon.csv";
    private const string EQUIP_CSV_PATH = "/ItemCSV/Equip.csv";
    private const string CRAFT_CSV_PATH = "/ItemCSV/Craft.csv";
    
    [Header("아이템 오브젝트")]
    [SerializeField]
    private NetworkPrefabRef _itemObjectPrefab;

    // 아이템 종류 별 딕셔너리로 구분됨. (추가 아이템 종류가 생기는 경우 딕셔너리 추가)
    private Dictionary<int, ItemProfile> _itemDictionary;
    
    // 아이템 팩토리
    private ItemFactory _itemFactory;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }
        Init();
    }

    private void Init()
    {
        _itemFactory = new ItemFactory(transform);
        
        // 데이터 로드 후 생성
        _itemDictionary = new Dictionary<int, ItemProfile>();
        
        // 음식 아이템
        var eatItemRawDataList = CSVLoader<EatItemRawData>.LoadCSV($"{Application.streamingAssetsPath}{FOOD_CSV_PATH}");
        foreach (var data in eatItemRawDataList)
        {
            var eatItem = _itemFactory.CreateItem(data);
            _itemDictionary[data.ID] = eatItem;
        }
        
        // 장비 아이템
        var equipmentItemRawDataList = CSVLoader<EquipmentItemRawData>.LoadCSV($"{Application.streamingAssetsPath}{EQUIP_CSV_PATH}");
        foreach (var data in equipmentItemRawDataList)
        {
            var useItem = _itemFactory.CreateItem(data);
            _itemDictionary[data.ID] = useItem;
        }
        
        // 무기 아이템
        var weaponItemRawData = CSVLoader<WeaponItemRawData>.LoadCSV($"{Application.streamingAssetsPath}{WEAPON_CSV_PATH}");
        foreach (var data in weaponItemRawData)
        {
            var weaponItem = _itemFactory.CreateItem(data);
            _itemDictionary[data.ID] = weaponItem;
        }
        
        // 도구 아이템
        var toolRawDataList = CSVLoader<UsableItemRawData>.LoadCSV($"{Application.streamingAssetsPath}{TOOL_CSV_PATH}");
        toolRawDataList.ForEach(x => x.ItemType = EItemType.Tool);
        
        // 씨앗 아이템
        var seedRawDataList = CSVLoader<UsableItemRawData>.LoadCSV($"{Application.streamingAssetsPath}{SEED_CSV_PATH}");
        seedRawDataList.ForEach(x => x.ItemType = EItemType.Seed);
        
        // 설치 아이템
        var craftRawDataList = CSVLoader<UsableItemRawData>.LoadCSV($"{Application.streamingAssetsPath}{CRAFT_CSV_PATH}");
        craftRawDataList.ForEach(x => x.ItemType = EItemType.Craft);
        
        var usableRawDataList = toolRawDataList;
        usableRawDataList.AddRange(seedRawDataList);
        usableRawDataList.AddRange(craftRawDataList);
        foreach (var data in usableRawDataList)
        {
            var usableItem = _itemFactory.CreateItem(data);
            _itemDictionary[data.ID] = usableItem;
        }
    }

    /// <summary>
    /// 아이템 조회 함수
    /// </summary>
    /// <param name="id">아이템 ID</param>
    public ItemProfile GetItem(int id)
    {
        return _itemDictionary.GetValueOrDefault(id);
    }

    public List<ItemDefinition> GetFoodIngredientList()
    {
        return _itemDictionary.Values
            .Where(itemInfo => itemInfo.ItemDefinition.IsIngredient)
            .Select(itemInfo => itemInfo.ItemDefinition)
            .ToList();
    }

    /// <summary>
    /// 아이템 생성(드랍)
    /// </summary>
    /// <param name="id">아이템 ID</param>
    /// <param name="quantity">수량</param>
    /// <param name="position">생성 위치</param>
    /// <param name="rotation">생성 시 각도</param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_CreateItemObject(int id, int quantity, float durability, Vector3 position, Quaternion rotation, string extraInfo = "", float pickableTime = 4f)
    {
        if (!Runner.IsServer)
        {
            return;
        }
        
        if (!_itemDictionary.TryGetValue(id, out ItemProfile item))
        {
            Debug.LogWarning($"없는 아이템입니다. ID: {id}");
            return;
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
                item.SpawnPosition = position;
                item.Durability = durability;
                item.ExtraInfo = extraInfo;
                item.PickableTime = pickableTime;
            });
    }
}
