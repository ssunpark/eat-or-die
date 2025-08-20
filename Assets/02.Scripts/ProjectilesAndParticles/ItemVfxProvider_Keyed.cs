using UnityEngine;

public class ItemVfxProvider_Keyed : MonoBehaviour, IAttackVfxProvider, IUseVfxProvider, IUseSfxProvider
{
    [Header("Attack Particle Keys")]
    [SerializeField] private string _windupKey;
    [SerializeField] private string _swingKey = "Swing_Default";
    [SerializeField] private string _hitKey = "Hit_Default";

    [Header("Use Particle Keys")]
    [SerializeField] private string _useStartKey;
    [SerializeField] private string _useLoopKey;
    [SerializeField] private string _useEndKey;
    [SerializeField] private string _useSuccessKey;
    [SerializeField] private string _useFailKey;
    
    [Header("Use Sound Keys")]
    [SerializeField] private string _useStartSoundEvent;
    [SerializeField] private string _useLoopSoundEvent;
    [SerializeField] private string _useEndSoundEvent;
    [SerializeField] private string _useSuccessSoundEvent;
    [SerializeField] private string _useFailSoundEvent;

    [Header("Use Spawn & Attach")]
    [SerializeField] private Transform _useSpawn;
    [SerializeField] private bool _mustbeChild;

    public string GetEffectKey(EAttackPhase phase) => phase switch
    {
        EAttackPhase.Windup => _windupKey,
        EAttackPhase.Swing => _swingKey,
        EAttackPhase.Hit => _hitKey,
        _ => null
    };
    public string GetEffectKey(EUsePhase phase) => phase switch
    {
        EUsePhase.Start => _useStartKey,
        EUsePhase.Loop => _useLoopKey,
        EUsePhase.End => _useEndKey,
        EUsePhase.Success => _useSuccessKey,
        EUsePhase.Fail => _useFailKey,
        _ => null
    };

    public bool MustBeChild => _mustbeChild;
    public Transform GetUseSpawnPoint() => _useSpawn != null ? _useSpawn : transform;
    
    public string GetSoundKey(EUsePhase phase) => phase switch
    {
        EUsePhase.Start => _useStartSoundEvent,
        EUsePhase.Loop => _useLoopSoundEvent,
        EUsePhase.End => _useEndSoundEvent,
        EUsePhase.Success => _useSuccessSoundEvent,
        EUsePhase.Fail => _useFailSoundEvent,
        _ => null
    };
}
