using System;
using DarkTonic.MasterAudio;
using Fusion;
using UnityEngine;
using Unity.Cinemachine;

// 수현짱!

public class SeedShopNpcInteractable : NetworkBehaviour, IInteractable
{
    public bool IsImmediate => true;
    public float InteractionDistanceOffset => 0.5f;

    [Header("Cinematic")]
    public CinemachineVirtualCameraBase CineCam;
    public static event Action PanelOpened;

    public UI_SeedShopPanel UI_SeedShopPanel;

    private void OnEnable()
    {
        UI_SeedShopPanel.OnClose += OnCloseShopPanel;
    }

    private void OnDisable()
    {
        UI_SeedShopPanel.OnClose -= OnCloseShopPanel;
    }
    public void Interact()
    {
        UI_SeedShopPanel.Open();
        MasterAudio.PlaySound3DAtTransform("NpcInteract", transform);

        CineCam.Priority = 200;

        PanelOpened?.Invoke();
    }

    private void OnCloseShopPanel()
    {
        if (CineCam != null)
        {
            CineCam.Priority = 10;
        }
    }
}
