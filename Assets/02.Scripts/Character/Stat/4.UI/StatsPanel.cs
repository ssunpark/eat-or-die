using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    [SerializeField] private StatUIItem[] _items;

    private StatManager _stat;

    public void BindLocal(Player localPlayer)
    {
        _stat = localPlayer.Stat;

        if (_items == null || _items.Length == 0)
            _items = GetComponentsInChildren<StatUIItem>(true);

        foreach (var it in _items)
            it.Bind(_stat);
    }

    public void Unbind()
    {
        foreach (var it in _items)
            it.Unbind();
        _stat = null;
    }

}
