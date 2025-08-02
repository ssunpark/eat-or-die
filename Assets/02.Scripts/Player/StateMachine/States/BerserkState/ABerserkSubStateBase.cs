using Fusion.Addons.FSM;

public abstract class ABerserkSubStateBase : State<ABerserkSubStateBase>
{
    protected PlayerFSM _controller;

    protected StatManager _stat;
    protected ResourceManager _resource;

    protected ABerserkSubStateBase(PlayerFSM controller)
    {
        _controller = controller;
        _stat = controller.Stat;
        _resource = controller.Resource;
    }
}