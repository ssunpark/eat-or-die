public enum EFarmingGroundState
{
    None = 0,          // 아무것도 안 된 상태
    Plowed,            // 밭만 간 상태
    Planted,           // 씨앗만 뿌려짐
    Watered,           // 물만 줌
    Growing            // 실제로 성장 진행 중
}