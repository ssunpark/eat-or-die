using UnityEngine;

[CreateAssetMenu(fileName = "NewSheetInfo", menuName = "Google Sheet/Sheet Info", order = 1)]
public class SheetInfoSO : ScriptableObject
{
    [Header("시트 이름 (저장될 파일명)")]
    public string SheetName;

    [Header("시트 GID")]
    public int SheetGID;

    [Header("저장 하위 폴더 (선택)")]
    public string SaveFolderName;
}