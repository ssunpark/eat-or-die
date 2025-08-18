using Fusion;

public class CharacterTraitNetworkSync : NetworkBehaviour
{
    [Networked, Capacity(8)] public NetworkArray<int> NetTraitLevels => default;

    private TraitManager _traitManager;

    public void Initialize(TraitManager manager)
    {
        _traitManager = manager;
        if (HasStateAuthority)
        {
            SyncAllTraits();
            _traitManager.OnTraitLeveledUp += HandleTraitLevelChanged;

            _traitManager.OnTraitLevelSet += HandleTraitLevelSet;
        }


    }

    private void HandleTraitLevelSet(ETraitType type, int newLv)
    {
        int index = (int)type;
        if (index >= 0 && index < NetTraitLevels.Length)
            NetTraitLevels.Set(index, newLv);
    }

    public void Dispose()
    {
        if (_traitManager == null) return;
        if (HasStateAuthority)
        {
            _traitManager.OnTraitLeveledUp -= HandleTraitLevelChanged;
            _traitManager.OnTraitLevelSet -= HandleTraitLevelSet;
        }
        _traitManager = null;
    }

    private void HandleTraitLevelChanged(ETraitType type, int delta)
    {
        if (!HasStateAuthority) return;

        var trait = _traitManager.GetTrait(type);
        int level = trait != null ? trait.Level : 0;
        int index = (int)type;
        if (index >= 0 && index < NetTraitLevels.Length)
            NetTraitLevels.Set(index, level);
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
