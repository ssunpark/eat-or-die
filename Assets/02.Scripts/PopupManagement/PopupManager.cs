using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopupManager : BehaviourSingleton<PopupManager>
{
    private readonly List<AUI_PopupBase> openedPopups = new();
    public bool HasOpenedPopup => openedPopups.Count > 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseLast();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryManager.Instance.OpenInventory();
        }

        if (!HasOpenedPopup && InputReader.Instance != null)
        {
            InputReader.Instance.GainControl();
        }
    }

    public void Register(AUI_PopupBase popup)
    {
        //if (!openedPopups.Contains(popup))
        openedPopups.Add(popup);
    }

    public void Unregister(AUI_PopupBase popup)
    {
        openedPopups.Remove(popup);
    }

    public void CloseLast()
    {
        if (openedPopups.Count > 0)
        {
            openedPopups[^1].Close();
        }
    }

    public bool IsOpen(EPopupType type)
    {
        return openedPopups.Any(popup => popup.Type == type);
    }

    public AUI_PopupBase GetOpenPopup(EPopupType type)
    {
        return openedPopups.FirstOrDefault(popup => popup.Type == type);
    }

    public IReadOnlyList<AUI_PopupBase> GetAllOpenPopups()
    {
        return openedPopups.AsReadOnly();
    }

    public void CloseAll()
    {
        IReadOnlyList<AUI_PopupBase> popups = GetAllOpenPopups();

        for (int i = popups.Count - 1; i >= 0; --i)
        {
            popups[i].Close();
        }
    }
}
