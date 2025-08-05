#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ItemSpawnerEditorWindow : EditorWindow
{
    private int _itemId = 0;
    private int _quantity = 1;
    private float _durability = 1;
    private Vector3 _spawnPosition = Vector3.zero;
    private Vector3 _spawnRotationEuler = Vector3.zero;

    private ItemData _lastSpawnedItemData;

    [MenuItem("Tools/Item Spawner")]
    public static void ShowWindow()
    {
        GetWindow<ItemSpawnerEditorWindow>("Item Spawner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Item Drop Test Tool", EditorStyles.boldLabel);

        _itemId = EditorGUILayout.IntField("Item ID", _itemId);
        _quantity = EditorGUILayout.IntField("Quantity", _quantity);
        _durability = EditorGUILayout.FloatField("Durability", _durability);
        _spawnPosition = EditorGUILayout.Vector3Field("Spawn Position", _spawnPosition);
        _spawnRotationEuler = EditorGUILayout.Vector3Field("Rotation (Euler)", _spawnRotationEuler);
        
        if (GUILayout.Button("Find Item"))
        {
            var itemManager = ItemManager.Instance;
            if (itemManager == null)
            {
                Debug.LogError("ItemManager.Instance 가 존재하지 않습니다. 씬에 ItemManager가 있어야 합니다.");
                return;
            }

            Quaternion rotation = Quaternion.Euler(_spawnRotationEuler);

            try
            {
                var item = itemManager.GetItem(_itemId);
                Debug.Log($"[EditorWindow] ID {_itemId} 아이템 조회 성공");
                _lastSpawnedItemData = item?.ItemData;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"아이템 조회 중 예외 발생: {ex.Message}");
                _lastSpawnedItemData = null;
            }
        }

        if (GUILayout.Button("Spawn Item"))
        {
            var itemManager = ItemManager.Instance;
            if (itemManager == null)
            {
                Debug.LogError("ItemManager.Instance 가 존재하지 않습니다. 씬에 ItemManager가 있어야 합니다.");
                return;
            }

            Quaternion rotation = Quaternion.Euler(_spawnRotationEuler);

            try
            {
                var durability = _durability == 0 ? itemManager.GetItem(_itemId).ItemData.MaxDurability :  _durability;
                itemManager.RPC_CreateItemObject(_itemId, _quantity, durability, _spawnPosition, rotation);
                Debug.Log($"[EditorWindow] ID {_itemId} 아이템 생성 성공");

                var item = itemManager.GetItem(_itemId);
                _lastSpawnedItemData = item?.ItemData;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"아이템 생성 중 예외 발생: {ex.Message}");
                _lastSpawnedItemData = null;
            }
        }

        if (_lastSpawnedItemData != null)
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("Spawned Item Info", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("ID", _lastSpawnedItemData.ID.ToString());
            EditorGUILayout.LabelField("Name", _lastSpawnedItemData.Name);
            EditorGUILayout.LabelField("HasDurability", _lastSpawnedItemData.HasDurability.ToString());
            EditorGUILayout.LabelField("IsIngredient", _lastSpawnedItemData.IsIngredient.ToString());
            EditorGUILayout.LabelField("Max Quantity", _lastSpawnedItemData.MaxQuantity.ToString());
            EditorGUILayout.LabelField("Max Durability", _lastSpawnedItemData.MaxDurability.ToString());
            EditorGUILayout.LabelField("Prefab", _lastSpawnedItemData.Prefab.name);
            EditorGUILayout.LabelField("Description");
            EditorGUILayout.TextArea(_lastSpawnedItemData.Description, GUILayout.Height(EditorGUIUtility.singleLineHeight * 5));

            if (_lastSpawnedItemData.Icon != null)
            {
                GUILayout.Label("Icon Preview");
                GUILayout.Label(AssetPreview.GetAssetPreview(_lastSpawnedItemData.Icon), GUILayout.Width(64), GUILayout.Height(64));
            }
            else
            {
                GUILayout.Label("아이콘 로딩 중 또는 없음");
            }
        }
    }
}
#endif
