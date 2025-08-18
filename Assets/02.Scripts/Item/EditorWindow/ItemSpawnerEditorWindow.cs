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

    private ItemDefinition _lastSpawnedItemDefinition;

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
                _lastSpawnedItemDefinition = item?.ItemDefinition;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"아이템 조회 중 예외 발생: {ex.Message}");
                _lastSpawnedItemDefinition = null;
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
                var durability = _durability == 0 ? itemManager.GetItem(_itemId).ItemDefinition.MaxDurability :  _durability;
                ItemProxySpawner.Instance.RPC_CreateItemObject(_itemId, _quantity, durability, _spawnPosition, rotation);
                Debug.Log($"[EditorWindow] ID {_itemId} 아이템 생성 성공");

                var item = itemManager.GetItem(_itemId);
                _lastSpawnedItemDefinition = item?.ItemDefinition;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"아이템 생성 중 예외 발생: {ex.Message}");
                _lastSpawnedItemDefinition = null;
            }
        }

        if (_lastSpawnedItemDefinition != null)
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("Spawned Item Info", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("ID", _lastSpawnedItemDefinition.ID.ToString());
            EditorGUILayout.LabelField("Name", _lastSpawnedItemDefinition.Name);
            EditorGUILayout.LabelField("ItemCategory", _lastSpawnedItemDefinition.Type.ToString());
            EditorGUILayout.LabelField("HasDurability", _lastSpawnedItemDefinition.HasDurability.ToString());
            EditorGUILayout.LabelField("IsIngredient", _lastSpawnedItemDefinition.IsIngredient.ToString());
            EditorGUILayout.LabelField("Max Quantity", _lastSpawnedItemDefinition.MaxQuantity.ToString());
            EditorGUILayout.LabelField("Max Durability", _lastSpawnedItemDefinition.MaxDurability.ToString());
            EditorGUILayout.LabelField("Prefab", _lastSpawnedItemDefinition.Prefab.name);
            EditorGUILayout.LabelField("Description");
            EditorGUILayout.TextArea(_lastSpawnedItemDefinition.Description, GUILayout.Height(EditorGUIUtility.singleLineHeight * 5));
            string extraDescription = "";
            foreach (var extra in _lastSpawnedItemDefinition.ExtraDescription)
            {
                extraDescription += extra + "\n";
            }
            EditorGUILayout.LabelField("ExtraDescription");
            EditorGUILayout.TextArea(extraDescription, GUILayout.Height(EditorGUIUtility.singleLineHeight * 5));

            if (_lastSpawnedItemDefinition.Icon != null)
            {
                GUILayout.Label("Icon Preview");
                GUILayout.Label(AssetPreview.GetAssetPreview(_lastSpawnedItemDefinition.Icon), GUILayout.Width(64), GUILayout.Height(64));
            }
            else
            {
                GUILayout.Label("아이콘 로딩 중 또는 없음");
            }
        }
    }
}
#endif
