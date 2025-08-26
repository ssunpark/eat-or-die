using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextSelectionSlider : MonoBehaviour
{
    public TextMeshProUGUI OptionText;
    [Header("코드에서 할당 중")]
    public List<string> Options;

    private int _selectedOption;

    public event Action<int> OnValueChanged;

    private void Start()
    {
        ChangeSelection();
    }

    public void OnPrevButtonClicked()
    {
        _selectedOption--;
        if (_selectedOption < 0)
        {
            _selectedOption = Options.Count - 1; 
        }

        ChangeSelection();
    }

    public void OnNextButtonClicked()
    {
        _selectedOption = (_selectedOption + 1) % Options.Count;

        ChangeSelection();
    }

    private void ChangeSelection()
    {
        OptionText.text = Options[_selectedOption];
        OnValueChanged?.Invoke(_selectedOption);
    }
}