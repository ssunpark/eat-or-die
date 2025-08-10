using Fusion;

public interface IAttackable
{
    NetworkObject NetworkObject { get; }
    void OnHitLocal(AttackInfo attack);
    void OnHitStateAuthority(AttackInfo attack);
    /*
    /// <param name="attack">공격 정보</param>
    /// <param name="attacker">공격하는 플레이어
    public void OnHitLocal(AttackInfo attack, NetworkObject attacker)
    {
        RPC_HitByAttack(attack, attacker);
        
        맞는 효과 재생

    }

    /// <summary>
    /// RPC sent to the State Authority when the attackable element has been hit locally first.
    /// </summary>
    /// <param name="attack">공격 정보</param>
    /// <param name="attacker">공격하는 플레이어
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_HitByAttack(AttackInfo attack, NetworkObject attacker)
    {
        OnHitStateAuthority(attack, attacker);
    }

    /// <summary>
    /// Abstract method for when being hit is offically confirmed by this object's State Authority.
    /// </summary>
    /// <param name="attack">Information about the attack.</param>
    /// <param name="attacker">The attacking player.</param>
    public void OnHitStateAuthority(AttackInfo attack, NetworkObject attacker)
    {
        // 실제 쳐맞는 로직 구현
    }
    */
}