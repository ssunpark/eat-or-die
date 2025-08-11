using Fusion;

public class SkillContext
{
    public readonly Player Player;
    
    public float MaxHunger => Player.Resource.MaxHunger;
    public float CurrentHunger => Player.Resource.CurrentHunger;
    public bool IsBerserk => Player.PlayerFSM.StateMachine.ActiveState is PlayerBerserkState;

    public SkillContext(Player player)
    {
        Player = player;
    }
}