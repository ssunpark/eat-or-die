using System.Collections.Generic;
using UnityEngine;

public class TraitsPanel : MonoBehaviour
{
    [SerializeField] private TraitUIEntry[] _entries;

    private TraitManager _traitManager;
    private List<CharacterTraitData> _dataList;

    public void BindLocal(Player localPlayer)
    {
        _traitManager = localPlayer.Trait;
        _dataList = localPlayer.TraitDataList;
        foreach (var e in _entries)
            e.Bind(_traitManager, _dataList);
    }

    public void Unbind()
    {
        foreach (var e in _entries)
            e.Unbind();

        _traitManager = null;
        _dataList = null;
    }
}
