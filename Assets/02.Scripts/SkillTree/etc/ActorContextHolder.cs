using Fusion;

public sealed class ActorContextHolder : NetworkBehaviour
{
    public SkillContext Context { get; private set; }

    public override void Spawned()
    {
        var player = GetComponent<Player>();
        Context = new SkillContext(player); // 필요 데이터 주입
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Context = null;
    }
}