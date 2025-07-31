using System;
using Fusion;

public class CharacterResourceNetworkSync : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnManaChanged))] public float NetCurrentMana { get; set; }
    [Networked, OnChangedRender(nameof(OnHungryChanged))] public float NetCurrentHungry { get; set; }

    private ResourceManager _resource;

    private Action<float, float> _onManaChangedHandler;
    private Action<float, float> _onHungryChangedHandler;
    private void OnHungryChanged()
    {
        _resource?.SetSatiety(NetCurrentHungry);
    }
    private void OnManaChanged()
    {
        _resource?.SetMana(NetCurrentMana);
    }
    public void Initialize(ResourceManager resource)
    {
        _resource = resource;

        _onManaChangedHandler = (cur, _) =>
        {
            if (HasStateAuthority) NetCurrentMana = cur;
        };
        _onHungryChangedHandler = (cur, _) =>
        {
            if (HasStateAuthority) NetCurrentHungry = cur;
        };

        _resource.OnHungerChanged += _onHungryChangedHandler;
        _resource.OnManaChanged += _onManaChangedHandler;
    }

    private void OnDisable()
    {
        if (_resource == null) return;

        if (_onHungryChangedHandler != null)
            _resource.OnHungerChanged -= _onHungryChangedHandler;
        if (_onManaChangedHandler != null)
            _resource.OnManaChanged -= _onManaChangedHandler;
    }
}