using Fusion;

public class SkillContext
{
    public readonly Player Player;
    
    public float MaxHunger => Player.Resource.MaxHunger;
    public float CurrentHunger => Player.Resource.CurrentHunger;
    public EPlayerState CurrentState => (EPlayerState)Player.PlayerFSM.StateMachine.ActiveStateId;

    public SkillContext(Player player)
    {
        Player = player;
    }
}