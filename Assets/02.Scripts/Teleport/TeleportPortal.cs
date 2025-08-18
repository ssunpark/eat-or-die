using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TeleportPortal : NetworkBehaviour, IInteractable
{
    public bool IsImmediate { get; } = true;
    
    public int StageIndex;
    
    public void Interact()
    {
        TeleportManager.Instance.PortalInteract(StageIndex);
    }
    
    public void OnTriggerExit(Collider other)
    {
        TeleportManager.Instance.ClosePortal();
    }

}
