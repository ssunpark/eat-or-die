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

#if UNITY_EDITOR
            Quaternion rotation = Quaternion.Euler(_spawnRotationEuler);

            try
            {
                _itemManager.CreateItemObject(_itemId, _quantity, _spawnPosition, rotation);
                Debug.Log($"[EditorWindow] ID {_itemId} 아이템 생성 성공");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"아이템 생성 중 예외 발생: {ex.Message}");
            }
#endif
        }
    }
}
#endif