using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.UI;

public class GlobalButtonSoundHandler : MonoBehaviour
{
    public void Awake()
    {
        // 현재 씬에 있는 모든 Button 찾기
        Button[] buttons = FindObjectsOfType<Button>(true);

        int count = 0;
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => MasterAudio.PlaySound("ButtonClick"));
        }
        
        // 현재 씬에 있는 모든 Toggle 찾기
        Toggle[] toggles = FindObjectsOfType<Toggle>(true);
        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener(_ => MasterAudio.PlaySound("ButtonClick"));
        }
    }
}