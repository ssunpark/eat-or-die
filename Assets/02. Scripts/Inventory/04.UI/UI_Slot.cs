using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Slot : MonoBehaviour
{
    private int _slotIndex;
    private Image _iconImage;
    private TextMeshProUGUI _quantityText;
    
    public void Initialize(int slotIndex)
    {
        _slotIndex = slotIndex;
    }
}
