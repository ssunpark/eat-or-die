using System;
using Fusion;

public class CharacterResourceNetworkSync : NetworkBehaviour
{
    [Networked] public float NetCurrentHealth { get; set; }
    [Networked] public float NetCurrentSatiety { get; set; }

    private ResourceManager _resource;

    private Action<float, float> _onHealthChangedHandler;
    private Action<float, float> _onSatietyChangedHandler;

    public void Initialize(ResourceManager resource)
    {
        _resource = resource;

        _onHealthChangedHandler = (cur, _) =>
        {
            if (HasStateAuthority) NetCurrentHealth = cur;
        };
        _onSatietyChangedHandler = (cur, _) =>
        {
            if (HasStateAuthority) NetCurrentSatiety = cur;
        };

        _resource.OnSatietyChanged += _onSatietyChangedHandler;
    }

    private void OnDisable()
    {
        if (_resource == null) return;

        if (_onSatietyChangedHandler != null)
            _resource.OnSatietyChanged -= _onSatietyChangedHandler;
    }
}