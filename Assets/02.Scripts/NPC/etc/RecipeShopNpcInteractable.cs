using System;
using DarkTonic.MasterAudio;
using Fusion;
using UnityEngine;
using Unity.Cinemachine;

public class RecipeShopNpcInteractable : NetworkBehaviour, IInteractable
{
    public bool IsImmediate => true;
    public float InteractionDistanceOffset => 0.5f;

    Player IInteractable.InteractingPlayer => _interactingPlayer;
    private Player _interactingPlayer;

    [Header("Cinematic")]
    public CinemachineVirtualCameraBase CineCam;
    public static event Action PanelOpened;

    public UI_RecipeShopPanel UI_RecipeShopPanel;

    private void OnEnable()
    {
        UI_RecipeShopPanel.OnClose += OnCloseShopPanel;
    }

    private void OnDisable()
    {
        UI_RecipeShopPanel.OnClose -= OnCloseShopPanel;
    }

    public void Interact()
    {
        UI_RecipeShopPanel.Open();
        // MasterAudio.PlaySound3DAtTransform("NpcInteract", transform);

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