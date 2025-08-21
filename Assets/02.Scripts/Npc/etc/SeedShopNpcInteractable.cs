using System;
using DarkTonic.MasterAudio;
using Fusion;
using UnityEngine;

using Unity.Cinemachine;

// 수현
public class SeedShopNpcInteractable : NetworkBehaviour, IInteractable
{
    public bool IsImmediate => true;
    public float InteractionDistanceOffset => 0.5f;

    [Header("Cinematic")]
    public CinemachineVirtualCameraBase CineCam; // 전용 VCam

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
        InputReader.Instance.ReleaseControl();
        MasterAudio.PlaySound3DAtTransform("NpcInteract", transform);

        CineCam.Priority = 200;
    }

    private void OnCloseShopPanel()
    {
        if (CineCam != null)
        {
            CineCam.Priority = 10;
        }
    }
}
