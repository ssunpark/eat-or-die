using System;
using DarkTonic.MasterAudio;
using Fusion;
using UnityEngine;
using Unity.Cinemachine;

public class ReviveShopNpcInteractable : MonoBehaviour, IInteractable
{
    public bool IsImmediate => true;
    public float InteractionDistanceOffset => 0.5f;

    Player IInteractable.InteractingPlayer => _interactingPlayer;
    private Player _interactingPlayer;

    [Header("Cinematic")]
    public CinemachineVirtualCameraBase CineCam;
    public static event Action PanelOpened;

    public UI_ReviveShopPanel UI_ReviveShopPanel;

    private void OnEnable()
    {
        UI_ReviveShopPanel.OnClose += OnCloseShopPanel;
    }

    private void OnDisable()
    {
        UI_ReviveShopPanel.OnClose -= OnCloseShopPanel;
    }

    public void Interact()
    {
        UI_ReviveShopPanel.Open();
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

    void IInteractable.Interact(Player from)
    {
        _interactingPlayer = from;
        Interact();
    }
}