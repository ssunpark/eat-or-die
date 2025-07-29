using TMPro;
using UnityEngine;

public class UI_NpcDialogue : MonoBehaviour
{
    public TextMeshProUGUI NpcDialogueText;
    
    public void Setup(string dialogue)
    {
        NpcDialogueText.text = dialogue;
    }
}