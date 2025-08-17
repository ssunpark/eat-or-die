using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TeleportPortal : NetworkBehaviour
{
    
    public void OnTriggerEnter(Collider other)
    {
        TeleportManager.Instance.PortalInteract();
    }

    public void OnTriggerExit(Collider other)
    {
        TeleportManager.Instance.PortalInteract();
    }


}
