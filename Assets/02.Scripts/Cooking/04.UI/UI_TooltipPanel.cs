using TMPro;
using UnityEngine;

public class UI_TooltipPanel : MonoBehaviour
{
    public TextMeshProUGUI tooltipText;

    public void SetText(string content)
    {
        if (tooltipText != null)
        {
            tooltipText.text = content;
        }
    }
}