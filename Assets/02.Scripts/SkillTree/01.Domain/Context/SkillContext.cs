using Fusion;

public class SkillContext
{
    public readonly Player Player;
    
    public float MaxHunger => Player.Resource.MaxHunger;
    public float CurrentHunger => Player.Resource.CurrentHunger;
    public bool IsBerserk => Player.PlayerFSM.StateMachine.ActiveStateId == (int)EPlayerState.Berserk;
    public bool IsIdle => Player.PlayerFSM.StateMachine.ActiveStateId == (int)EPlayerState.Idle;

    public SkillContext(Player player)
    {
        Player = player;
    }
}