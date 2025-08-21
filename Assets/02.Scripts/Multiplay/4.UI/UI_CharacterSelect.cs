using TMPro;
using UnityEngine;

public class UI_CharacterSelect : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.World;
    [SerializeField] private GameObject _container;
    [SerializeField] private GameObject _chracterProfilePrefab;

    public void Refresh()
    { 
        
    }
}
