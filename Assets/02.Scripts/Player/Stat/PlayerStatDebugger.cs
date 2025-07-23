using UnityEngine;

public class PlayerStatDebugger : MonoBehaviour
{
    private StatManager _statManager;

    public void Bind(StatManager statManager)
    {
        _statManager = statManager;
    }

    private void OnGUI()
    {
        // if (_statManager == null) return;
        //
        // GUI.BeginGroup(new Rect(400, 10, 300, Screen.height));
        // GUILayout.Label("<b><size=14>Player Stats</size></b>");
        //
        // foreach (var kvp in _statManager.GetStatSnapshot())
        // {
        //     GUILayout.Label($"{kvp.Key}: {kvp.Value:F2}");
        // }
        //
        // GUI.EndGroup();
    }
}
