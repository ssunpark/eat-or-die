using UnityEngine;

public class PlayerStatDebugger : MonoBehaviour
{
    private PlayerStat _stat;

    private void Awake()
    {
        _stat = GetComponent<PlayerStat>();
    }

    private void OnGUI()
    {
        if (_stat == null || _stat.StatDictionary == null) return;

        GUI.BeginGroup(new Rect(10, 10, 300, Screen.height));
        GUILayout.Label("<b><size=14>Player Stats</size></b>");

        foreach (var entry in _stat.StatDictionary)
        {
            GUILayout.Label($"{entry.Key}: {entry.Value.TotalStat:F2}");
        }

        GUI.EndGroup();
    }
}
