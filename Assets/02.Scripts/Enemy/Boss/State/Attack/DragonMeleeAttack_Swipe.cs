public class DragonMeleeAttack_Swipe : DragonSubStateBase
{
    private DragonStateParameterSet.SwipeParams _swipeParams;
    private bool _hasAttacked;

    public DragonMeleeAttack_Swipe(
        DragonController controller,
        IParentState parent,
        DragonStateParameterSet.SwipeParams swipe)
        : base(controller, parent)
    {
        _swipeParams = swipe;
    }

    protected override void OnEnterState()
    {
        _hasAttacked = false;
        
        Controller.Animator.SetTrigger("Attack_Swipe");
    }

    protected override void OnFixedUpdate()
    {
        // ParentState.OnSubStateComplete();
        if (!_hasAttacked && Machine.StateTime >= _swipeParams.SwipeDuration)
        {
            _hasAttacked = true;
        }
        
        if (_hasAttacked && Machine.StateTime >= _swipeParams.SwipeDuration + 0.3f)
        {
            ParentState.OnSubStateComplete();
        }
    }
}