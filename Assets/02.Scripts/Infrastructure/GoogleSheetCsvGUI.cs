#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class GoogleSheetCsvGUI
{
    private static Vector2 _previewScroll;     // 파일별 아님, 공용 하나만
    private const float PREVIEW_HEIGHT = 320f; // 원하는 고정 높이
    // Window에서는 true로, Inspector에서는 false로
    public static void DrawBody(GoogleSheetCSVLoader loader,
                                SerializedObject so,
                                ref Vector2 listScroll,
                                Dictionary<string, string> previewByPath,
                                bool asWindow)
    {
        if (!loader)
        {
            EditorGUILayout.HelpBox("씬에서 GoogleSheetCSVLoader를 찾거나 Drag & Drop 해주세요.", MessageType.Warning);
            return;
        }

        so.Update();

        if (asWindow)
        {
            EditorGUILayout.HelpBox("씬의 GoogleSheetCSVLoader를 선택하고 [로드]를 누르면 저장 경로와 저장된 내용이 바로 표시됩니다.", MessageType.Info);
        }

        // 저장 루트 표시(변경은 컴포넌트에서)
        using (new EditorGUI.DisabledScope(true))
        {
            var saveRootProp = so.FindProperty("saveRoot");
            if (saveRootProp != null)
                EditorGUILayout.PropertyField(saveRootProp, new GUIContent("Save Root (set on component)"));
        }

        // 시트 리스트
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sheets", EditorStyles.boldLabel);

        var sheetInfosProp = so.FindProperty("sheetInfos");
        if (sheetInfosProp == null)
        {
            EditorGUILayout.HelpBox("sheetInfos 직렬화 필드를 찾을 수 없습니다.", MessageType.Error);
            return;
        }

        listScroll = EditorGUILayout.BeginScrollView(listScroll);
        for (int i = 0; i < sheetInfosProp.arraySize; i++)
        {
            var elem = sheetInfosProp.GetArrayElementAtIndex(i);

            // ScriptableObject(SheetInfoSO) 리스트를 가정
            var soRef = elem.objectReferenceValue as SheetInfoSO;
            if (!soRef)
            {
                EditorGUILayout.HelpBox("SheetInfoSO 참조가 비어 있습니다.", MessageType.Warning);
                continue;
            }

            string root = GetSaveRootPathFromLoader(loader);
            string safeFolder = string.IsNullOrWhiteSpace(soRef.SaveFolderName)
                ? null
                : InvokeSanitizeOnLoader(soRef.SaveFolderName);
            string safeName = InvokeSanitizeOnLoader(soRef.SheetName);

            string dir = string.IsNullOrEmpty(safeFolder) ? root : Path.Combine(root, safeFolder);
            string path = Path.Combine(dir, $"{safeName}.csv");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.ObjectField("Sheet", soRef, typeof(SheetInfoSO), false);
            EditorGUILayout.LabelField($"Name: {soRef.SheetName} (GID: {soRef.SheetGID})");
            EditorGUILayout.LabelField("Save Path:");
            var pathStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = false };
            EditorGUILayout.TextArea(path, pathStyle, GUILayout.MinHeight(22), GUILayout.MaxHeight(60));

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("로드", GUILayout.Width(100)))
                {
                    _ = LoadOneAsync(loader, soRef.SheetGID, soRef.SheetName, soRef.SaveFolderName, path, previewByPath);
                }

                if (GUILayout.Button("폴더 열기", GUILayout.Width(100)))
                {
                    EnsureDirectory(dir);
                    EditorUtility.RevealInFinder(dir);
                }

                GUILayout.FlexibleSpace();
            }

            if (previewByPath.TryGetValue(path, out var previewText))
            {
                EditorGUILayout.Space(2);
                EditorGUILayout.LabelField("Saved Content Preview:");

                var previewStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = false };
                _previewScroll = EditorGUILayout.BeginScrollView(_previewScroll);
                EditorGUILayout.TextArea(previewText ?? string.Empty, previewStyle, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.MinWidth(previewStyle.fontSize), GUILayout.MinHeight(previewStyle.fontSize));
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        so.ApplyModifiedProperties();
    }

    // --- 내부 유틸들 (윈도우/인스펙터 공용) ---

    private static string GetSaveRootPathFromLoader(GoogleSheetCSVLoader d)
    {
        var saveRootField = typeof(GoogleSheetCSVLoader).GetField("saveRoot",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        string root;
        if (saveRootField != null)
        {
            var enumVal = saveRootField.GetValue(d)?.ToString();
            root = (enumVal == "PersistentData") ? Application.persistentDataPath : Application.streamingAssetsPath;
        }
        else
        {
            root = Application.streamingAssetsPath;
        }
        return root;
    }

    private static string InvokeSanitizeOnLoader(string raw)
    {
        var mi = typeof(GoogleSheetCSVLoader).GetMethod("SanitizeFileName",
            BindingFlags.Static | BindingFlags.NonPublic);
        if (mi != null)
        {
            return (string)mi.Invoke(null, new object[] { raw });
        }
        return raw?.Trim();
    }

    private static void EnsureDirectory(string dir)
    {
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }

    private static async Task LoadOneAsync(GoogleSheetCSVLoader loader,
                                           int gid, string rawName, string rawFolder, string expectedPath,
                                           Dictionary<string, string> previewByPath)
    {
        try
        {
            await loader.DownloadToLocalAsync(gid, rawName, rawFolder);

            if (!File.Exists(expectedPath))
            {
                previewByPath[expectedPath] = "(파일이 없습니다)";
            }
            else
            {
                string text;
                using (var fs = new FileStream(expectedPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                    text = await sr.ReadToEndAsync();

                const int maxBytes = 100 * 1024; // 100KB
                if (Encoding.UTF8.GetByteCount(text) > maxBytes)
                {
                    var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                    var sb = new StringBuilder();
                    int bytes = 0;
                    foreach (var line in lines)
                    {
                        var add = line + "\n";
                        int addBytes = Encoding.UTF8.GetByteCount(add);
                        if (bytes + addBytes > maxBytes) break;
                        sb.Append(add);
                        bytes += addBytes;
                    }
                    sb.Append("\n... (preview truncated)");
                    text = sb.ToString();
                }

                previewByPath[expectedPath] = text;
            }
        }
        catch (Exception e)
        {
            previewByPath[expectedPath] = $"로드 실패: {e.Message}";
            Debug.LogError(e);
        }
    }
}
#endif
