using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetCSVLoader : MonoBehaviour
{
    public enum SaveRoot
    {
        StreamingAssets, // 개발 중 빠른 접근 (모바일/콘솔에선 쓰기 제한)
        PersistentData   // 런타임 쓰기 안전 (권장)
    }

    [Header("고정값")]
    [SerializeField]
    private string sheetId = "1EpQeuOOloDepp55rhYwWBBtJFLsiAq381-Kf9nTTQvY";

    [Header("저장 루트 선택")]
    [SerializeField]
    private SaveRoot saveRoot = SaveRoot.StreamingAssets;

    [Header("시트 정보")]
    [SerializeField]
    private List<SheetInfoSO> sheetInfos;

    public async Task DownloadToLocalAsync(int gid, string filename, string folderName = null,
        CancellationToken ct = default)
    {
        string url = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv&gid={gid}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            var op = www.SendWebRequest();

            while (!op.isDone)
            {
                if (ct.IsCancellationRequested)
                {
                    www.Abort();
                    ct.ThrowIfCancellationRequested();
                }
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
                throw new Exception($"CSV 다운로드 실패: {www.error}\nURL: {url}");

            string root = (saveRoot == SaveRoot.PersistentData)
                ? Application.persistentDataPath
                : Application.streamingAssetsPath;

            string sub = string.IsNullOrWhiteSpace(folderName)
                ? root
                : Path.Combine(root, SanitizeFileName(folderName));

            if (!Directory.Exists(sub))
                Directory.CreateDirectory(sub);

            string safeFile = SanitizeFileName(filename);
            string path = Path.Combine(sub, $"{safeFile}.csv");
            
            string csvText = www.downloadHandler.text;
            string[] lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            if (lines.Length > 1)
            {
                var list = new List<string>(lines);
                list.RemoveAt(1);
                csvText = string.Join("\n", list);
            }

            await File.WriteAllBytesAsync(path, Encoding.UTF8.GetBytes(csvText), ct);

            Debug.Log($"CSV 저장 완료: {path}");
        }
    }

    public async void DownloadCSV()
    {
        try
        {
            foreach (var sheetInfo in sheetInfos)
            {
                await DownloadToLocalAsync(sheetInfo.SheetGID, sheetInfo.SheetName, sheetInfo.SaveFolderName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "sheet";

        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');

        return name.Trim();
    }
}
