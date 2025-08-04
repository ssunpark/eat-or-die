using System;
using TMPro;
using UnityEngine;

public class ClassSelector : MonoBehaviour
{
    [HideInInspector] public int CurrentIndex = 0;
    public string[] Options;
    public TextMeshProUGUI ClassText;
    public void Next()
    {
        CurrentIndex = (CurrentIndex + 1) % Options.Length;
        SetString(CurrentIndex);
    }

    public void Prev()
    {
        CurrentIndex = (CurrentIndex - 1 + Options.Length) % Options.Length;
        SetString(CurrentIndex);
    }

    public void SetString(int index)
    {
        ClassText.text = Options[index];
    }
}
