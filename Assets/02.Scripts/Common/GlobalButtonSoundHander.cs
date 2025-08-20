using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.UI;

public class GlobalButtonSoundHander : MonoBehaviour
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
    }
}