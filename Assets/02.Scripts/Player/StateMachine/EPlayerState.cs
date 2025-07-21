public enum EPlayerState : byte
{
    Idle,
    Move,
    Attack,
    Skill,
    Interact,
    Cooking,
    Farming,
    Harvesting,
    CarryingCorpse,
    Down,
    Dead,
    Hit,
    // 여기서부터는 쓸지 안 쓸지 모르는데 그냥 추가해놓음..
    Fishing,
    Mine,
}