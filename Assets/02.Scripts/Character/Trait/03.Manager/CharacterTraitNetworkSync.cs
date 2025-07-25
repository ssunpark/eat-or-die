using Fusion;

public class CharacterTraitNetworkSync : NetworkBehaviour
{
    [Networked, Capacity(8)] public NetworkArray<int> NetTraitLevels => default;

    private TraitManager _traitManager;

    public void Initialize(TraitManager manager)
    {
        _traitManager = manager;
        if (HasStateAuthority)
            SyncAllTraits();
    }

    public void SyncAllTraits()
    {
        foreach (var kvp in _traitManager.GetTraitSnapshot())
        {
            int index = (int)kvp.Key;
            NetTraitLevels.Set(index, kvp.Value);
        }
    }
}
