#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GoogleSheetCSVLoader))]
public class GoogleSheetCSVLoaderInspector : Editor
{
    private SerializedObject so;
    private Vector2 listScroll;
    private readonly Dictionary<string, string> previewByPath = new();

    private void OnEnable()
    {
        so = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        // 기본 필드
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== CSV Loader Tools ===", EditorStyles.boldLabel);

        // 공용 드로어 호출 (asWindow = false)
        GoogleSheetCsvGUI.DrawBody((GoogleSheetCSVLoader)target, so, ref listScroll, previewByPath, false);
    }
}
#endif