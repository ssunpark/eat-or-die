#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ItemSpawnerEditorWindow : EditorWindow
{
    private ItemManager _itemManager;

    private int _itemId = 0;
    private int _quantity = 1;
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

        _itemManager = (ItemManager)EditorGUILayout.ObjectField("Item Manager", _itemManager, typeof(ItemManager), true);

        _itemId = EditorGUILayout.IntField("Item ID", _itemId);
        _quantity = EditorGUILayout.IntField("Quantity", _quantity);
        _spawnPosition = EditorGUILayout.Vector3Field("Spawn Position", _spawnPosition);
        _spawnRotationEuler = EditorGUILayout.Vector3Field("Rotation (Euler)", _spawnRotationEuler);

        if (GUILayout.Button("Spawn Item"))
        {
            if (_itemManager == null)
            {
                Debug.LogError("ItemManager를 할당하세요.");
                return;
            }

            Quaternion rotation = Quaternion.Euler(_spawnRotationEuler);

            try
            {
                _itemManager.CreateItemObject(_itemId, _quantity, _spawnPosition, rotation);
                Debug.Log($"[EditorWindow] ID {_itemId} 아이템 생성 성공");

                // 아이템 정보 저장
                var item = _itemManager.GetItem(_itemId);
                _lastSpawnedItemData = item?.ItemData;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"아이템 생성 중 예외 발생: {ex.Message}");
                _lastSpawnedItemData = null;
            }
        }

        // 아이템 정보 출력
        if (_lastSpawnedItemData != null)
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("Spawned Item Info", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("ID", _lastSpawnedItemData.ID.ToString());
            EditorGUILayout.LabelField("Name", _lastSpawnedItemData.Name);
            EditorGUILayout.LabelField("Description", _lastSpawnedItemData.Description);
            EditorGUILayout.LabelField("Max Quantity", _lastSpawnedItemData.MaxQuantity.ToString());

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
