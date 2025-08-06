public enum EPlayerState : byte
{
    Idle,
    Move,
    Attack,
    Interact,
    UseItem,
    Cooking,
    Hit,
    Dead,
    Berserk,
    Recover,
    Corpse,
    
    // 여기서부터는 쓸지 안 쓸지 모르는데 그냥 추가해놓음..
    Fishing,
}