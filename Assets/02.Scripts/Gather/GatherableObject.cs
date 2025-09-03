using DarkTonic.MasterAudio;
using Fusion;
using UnityEngine;

public class GatherableObject : NetworkBehaviour, IInteractable
{
    [Networked]
    public int GatherableID { get; set; }

    [SerializeField] private string _gatherSound = "PlantPop";

    private Player _interactingPlayer;

    public bool IsImmediate => false;

    public float InteractionDistanceOffset => 0.2f;

    Player IInteractable.InteractingPlayer => _interactingPlayer;

    public void Interact()
    {
        var drops = GatherableManager.Instance.GetDrops(GatherableID);
        foreach (var drop in drops)
        {
            int count = Random.Range(drop.MinCount, drop.MaxCount + 1);
            if (count > 0)
            {
                ItemProxySpawner.Instance.RPC_CreateItemObject(drop.ItemID, count, 1f, transform.position, Quaternion.identity);
            }
        }
        MasterAudio.PlaySound3DAtTransform(_gatherSound, transform);
        RPC_Despawn();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_Despawn()
    {
        if (!Runner.IsServer)
        {
            return;
        }
        Runner.Despawn(Object);
    }

    void IInteractable.Interact(Player from)
    {
        _interactingPlayer = from;
        Interact();
    }
}