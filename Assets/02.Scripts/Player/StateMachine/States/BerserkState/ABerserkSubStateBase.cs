using Fusion.Addons.FSM;

public abstract class ABerserkSubStateBase : State<ABerserkSubStateBase>
{
    protected PlayerController _controller;

    protected StatManager _stat;
    protected ResourceManager _resource;

    protected ABerserkSubStateBase(PlayerController controller)
    {
        _controller = controller;
        _stat = controller.Stat;
        _resource = controller.Resource;
    }
}