using Fusion;
using UnityEngine;
// 수현
public class CookingPotInteractable : NetworkBehaviour, IInteractable
{
    public bool IsImmediate => true;

    public float InteractionDistanceOffset => 0f;

    public UI_CookingPanel CookingPanelUI;

    private bool _isCooking; //jh
    
    public void Interact()
    {
        CookingPanelUI.Open();
        InputReader.Instance.ReleaseControl();
        CookingManager.Instance.SetCurrentCookingPot(this); // 로컬의 쿠킹 매니저에 현재 CookingPot을 등록합니다.
    }

    //jh
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_StartCooking(PlayerRef player, RpcInfo info = default)
    {
        if (_isCooking)
        {
            // CookingPot을 이미 사용중인 경우
            CookingManager.Instance.Rpc_CookingPotAlreadyUse(info.Source);
            return;
        }
        else
        {
            // 요리가 가능한 경우
            _isCooking = true;
            CookingManager.Instance.Rpc_StartCooking(info.Source);
            PlayerInfoManager.PlayerControllers[player].RequestState(EPlayerState.Cooking);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_EndCooking()
    {
        _isCooking = false;
    }
}
