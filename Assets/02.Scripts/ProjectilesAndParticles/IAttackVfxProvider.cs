using UnityEngine;

public enum EAttackPhase { Windup, Swing, Hit}
public enum EUsePhase { Start, Loop, End, Success, Fail }

public interface IAttackVfxProvider
{
    string GetEffectKey(EAttackPhase phase);
}

public interface IUseVfxProvider
{
    string GetEffectKey(EUsePhase phase); 
    bool MustBeChild { get; }
    Transform GetUseSpawnPoint();
}

public interface IUseSfxProvider
{
    string GetSoundKey(EUsePhase phase);
}