using Fusion;

public abstract class CharacterBase : NetworkBehaviour, IStatUser
{
    public StatManager Stat { get; protected set; }
    public ResourceManager Resource { get; protected set; }

    public TraitManager Trait { get; protected set; }

    public virtual void InitializeCharacter(IStatDataRepository statRepo, ITraitDataRepository traitRepo, ECharacterType characterType)
    {
        Stat = new StatManager(statRepo, characterType);
        Trait = new TraitManager(traitRepo ?? new MockTraitDataRepository(), Stat);
        Resource = new ResourceManager(Stat);
    }

}
