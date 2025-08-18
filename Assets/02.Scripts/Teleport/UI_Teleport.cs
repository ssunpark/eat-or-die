using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Teleport : AUI_PopupBase
{
    public override EPopupType Type { get; } = EPopupType.Default;
    
    [SerializeField] private List<Toggle> _selectToggle;

    private void Start()
    {
        TeleportManager.Instance.OnInteractPortal += OpenTeleportUI;
        TeleportManager.Instance.OnExitPortal += CloseTeleportUI;
        base.Close();
    }

    public void OpenTeleportUI()
    { 
        base.Open();
    }

    public void CloseTeleportUI()
    {
        base.Close();
    }
    
    public void OnClickToggle(int index)
    {
        TeleportManager.Instance.DestinationStage = index;
    }

    public void OnClickConfirm()
    {
        TeleportManager.Instance.Teleport();
        base.Close();
    }
}
