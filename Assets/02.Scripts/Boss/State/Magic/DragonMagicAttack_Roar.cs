public class DragonMagicAttack_Roar : DragonSubStateBase
{
    private DragonStateParameterSet.RoarParams _roarParams;
    private bool _onFired;

    public DragonMagicAttack_Roar(
        DragonController controller,
        IParentState parentState,
        DragonStateParameterSet.RoarParams roarParams)
        : base(controller, parentState)
    {
        _roarParams = roarParams;
    }

    protected override void OnEnterState()
    {
        Controller.Lock();
        Controller.Animator.SetBool("IsMove", false);
        Controller.Animator.SetBool("Attack_Roar", true);
    }

    protected override void OnFixedUpdate()
    {
        if (!_onFired && Machine.StateTime >= _roarParams.FireTime)
        {
            _onFired = true;
            // 발사
            float interval = _roarParams.Duration / _roarParams.Count;
            Controller.RoarExplosion.Reset(_roarParams.Radius, _roarParams.Count, interval);
            return;
        }
        
        if (Machine.StateTime >= _roarParams.FireTime + _roarParams.Duration)
        {
            Controller.Animator.SetBool("Attack_Roar", false);
        }
        
        if (!Controller.IsLocked)
        {
            ParentState.OnSubStateComplete();
        }

        return;
    }

    protected override void OnExitState()
    {
        _onFired = false;
    }
}