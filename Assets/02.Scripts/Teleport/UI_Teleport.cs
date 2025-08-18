using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Teleport : AUI_PopupBase
{
    public override EPopupType Type { get; } = EPopupType.Default;
    
    [SerializeField] private List<Toggle> _selectToggle;

    private void Start()
    {
        TeleportManager.Instance.OnInteractPortal += ToggleTeleport;
    }

    public void ToggleTeleport()
    {
        if (gameObject.activeInHierarchy)
        {
            base.Close();
        }
        else
        {
            base.Open();
        }
    }
    
    public void OnClickToggle(int index)
    {
        foreach (Toggle toggle in _selectToggle)
        {
            toggle.isOn = false;
        }
        _selectToggle[index].isOn = true;
        TeleportManager.Instance.DestinationStage = index;
    }

    public void OnClickConfirm()
    {
        TeleportManager.Instance.Teleport();
    }
}
