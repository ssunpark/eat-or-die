using UnityEngine;
using Fusion.Addons.FSM;
public class BerserkIdle : ABerserkSubStateBase
{

    public BerserkIdle(PlayerFSM controller) : base(controller) { }

    public override void OnActionMoment()
    {
        Debug.Log("뿡");
    }

    protected override void OnEnterStateRender()
    {
        Anim.CrossFadeInFixedTime("Berserk Start", AnimTransitionLength);
    }

    protected override void OnFixedUpdate()
    {
        if (!_fsm.HasStateAuthority) return;
        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths["Berserk Start"])
        {
            Machine.ForceActivateState<BerserkChase>();
        }
    }
}