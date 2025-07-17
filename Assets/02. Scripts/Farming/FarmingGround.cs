using Fusion;
using UnityEngine;

public class FarmingGround : NetworkBehaviour
{
    [Networked]
    public EFarmingGroundState State { get; set; }
    
    [SerializeField]
    private GameObject _baseGround;
    
    [SerializeField]
    private GameObject _plowedGround;

    public override void Spawned()
    {
        State = EFarmingGroundState.None;
        _baseGround.SetActive(true);
        _plowedGround.SetActive(false);
    }

    public void Plow()
    {
        // 밭 갈기
        State = EFarmingGroundState.Plowed;
        _baseGround.SetActive(false);
        _plowedGround.SetActive(true);
    }

    public void Water()
    {
        if (State != EFarmingGroundState.Plowed)
        {
            return;
        }
        State = EFarmingGroundState.Watered;
        // 머티리얼 변경
    }
}