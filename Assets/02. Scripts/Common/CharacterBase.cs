using Fusion;

public abstract class CharacterBase : NetworkBehaviour, IStatUser
{
    public StatManager Stat { get; private set; }
    public ResourceManager Resource { get; private set; }

    public virtual void InitializeStat(IStatDataRepository repo)
    {
        Stat = new StatManager(repo);
        Resource = new ResourceManager(Stat);
    }
}
