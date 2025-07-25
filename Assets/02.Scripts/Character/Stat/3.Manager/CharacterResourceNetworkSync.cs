using System;
using Fusion;

public class CharacterResourceNetworkSync : NetworkBehaviour
{
    [Networked] public float NetCurrentMana { get; set; }
    [Networked] public float NetCurrentSatiety { get; set; }

    private ResourceManager _resource;

    private Action<float, float> _onManaChangedHandler;
    private Action<float, float> _onSatietyChangedHandler;

    public void Initialize(ResourceManager resource)
    {
        _resource = resource;

        _onManaChangedHandler = (cur, _) =>
        {
            if (HasStateAuthority) NetCurrentMana = cur;
        };
        _onSatietyChangedHandler = (cur, _) =>
        {
            if (HasStateAuthority) NetCurrentSatiety = cur;
        };

        _resource.OnSatietyChanged += _onSatietyChangedHandler;
        _resource.OnManaChanged += _onManaChangedHandler;
    }

    private void OnDisable()
    {
        if (_resource == null) return;

        if (_onSatietyChangedHandler != null)
            _resource.OnSatietyChanged -= _onSatietyChangedHandler;
        if (_onManaChangedHandler != null)
            _resource.OnManaChanged -= _onManaChangedHandler;
    }
}