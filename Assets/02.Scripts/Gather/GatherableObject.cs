using DarkTonic.MasterAudio;
using Fusion;
using UnityEngine;

public class GatherableObject : NetworkBehaviour, IInteractable
{
    [Networked]
    public int GatherableID { get; set; }

    [SerializeField] private string _gatherSound = "PlantPop";
    [SerializeField] private float _respawnDelay = 10f;

    [Networked, OnChangedRender(nameof(OnActiveStateChanged))]
    private NetworkBool _isActive { get; set; } = true;
    [Networked]
    private TickTimer _respawnTimer { get; set; }

    private Player _interactingPlayer;
    private Renderer[] _renderers;
    private Collider[] _colliders;

    public bool IsImmediate => false;

    public float InteractionDistanceOffset => 0.2f;

    Player IInteractable.InteractingPlayer => _interactingPlayer;

    public override void Spawned()
    {
        _renderers = GetComponentsInChildren<Renderer>(true);
        _colliders = GetComponentsInChildren<Collider>();
        OnActiveStateChanged();
    }

    public void Interact()
    {
        if (!_isActive)
            return;

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
        RPC_StartRespawn();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer)
            return;

        if (!_isActive && _respawnTimer.Expired(Runner))
        {
            _isActive = true;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_StartRespawn()
    {
        if (!Runner.IsServer)
            return;

        _isActive = false;
        _respawnTimer = TickTimer.CreateFromSeconds(Runner, _respawnDelay);
    }

    private void OnActiveStateChanged()
    {
        bool active = _isActive;
        if (_renderers != null)
        {
            foreach (var r in _renderers)
                r.enabled = active;
        }
        if (_colliders != null)
        {
            foreach (var c in _colliders)
                c.enabled = active;
        }
    }

    void IInteractable.Interact(Player from)
    {
        _interactingPlayer = from;
        Interact();
    }
}