#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GoogleSheetCSVLoaderWindow : EditorWindow
{
    [SerializeField] private GoogleSheetCSVLoader loader; // 창에 직접 드래그도 가능
    private SerializedObject so;
    private Vector2 listScroll;
    private readonly Dictionary<string, string> previewByPath = new();

    [MenuItem("Tools/Google Sheet CSV Loader")]
    public static void ShowWindow() => GetWindow<GoogleSheetCSVLoaderWindow>("Sheet CSV Loader");

    private void OnEnable()
    {
        if (!loader) loader = FindObjectOfType<GoogleSheetCSVLoader>();
        so = loader ? new SerializedObject(loader) : null;
    }

    private void OnGUI()
    {
        // 로더 선택
        var newLoader = (GoogleSheetCSVLoader)EditorGUILayout.ObjectField("Loader (in Scene)", loader, typeof(GoogleSheetCSVLoader), true);
        if (newLoader != loader)
        {
            loader = newLoader;
            so = loader ? new SerializedObject(loader) : null;
            previewByPath.Clear();
        }

        if (!loader)
        {
            if (GUILayout.Button("Find Loader in Scene"))
            {
                loader = FindObjectOfType<GoogleSheetCSVLoader>();
                so = loader ? new SerializedObject(loader) : null;
            }
            GoogleSheetCsvGUI.DrawBody(null, null, ref listScroll, previewByPath, true);
            return;
        }

        if (so == null) so = new SerializedObject(loader);

        // 공용 드로어 호출 (asWindow = true)
        GoogleSheetCsvGUI.DrawBody(loader, so, ref listScroll, previewByPath, true);
    }
}
#endif