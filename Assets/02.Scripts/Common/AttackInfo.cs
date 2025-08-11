using Fusion;
using UnityEngine;

public struct AttackInfo : INetworkStruct
{
    public float MagicDamage;

    public float MeleeDamage;

    public Vector3 KnockbackVector;

    public float HitRecoveryTime;

    public float BossDamageMultiplier;

    public float TotalDamageMultiplier;

    public NetworkObject Attacker;

}