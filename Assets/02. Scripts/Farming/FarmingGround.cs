using Fusion;

public class FarmingGround : NetworkBehaviour
{
    // 땅에 심어진 작물에 대한 ID
    private int _seedID;

    private EFarmingGroundState _state;

    public override void Spawned()
    {
        _state = EFarmingGroundState.None;
    }

    public void Plow()
    {
        // 밭 갈기
        _state = EFarmingGroundState.Plowed;
    }

    public void Water()
    {
        if (_state == EFarmingGroundState.Plowed)
        {
            _state = EFarmingGroundState.Watered;
        }
        else if (_state == EFarmingGroundState.Planted)
        {
            Grow();
        }
    }

    public void Plant(int seedID)
    {
        _seedID = seedID;
        
        if (_state == EFarmingGroundState.Plowed)
        {
            _state = EFarmingGroundState.Planted;
        }
        else if (_state == EFarmingGroundState.Watered)
        {
            Grow();
        }
    }

    private void Grow()
    {
        _state = EFarmingGroundState.Growing;
        // 식물 자라남
    }
}