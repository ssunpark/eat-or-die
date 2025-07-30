using Fusion.Addons.FSM;

public abstract class APlayerStateBase : State<APlayerStateBase>
{
    protected PlayerController _controller;
    protected StatManager _stat;
    protected ResourceManager _resource;

    public APlayerStateBase(PlayerController controller)
    {
        _controller = controller;
        _stat = controller.Stat;
        _resource = controller.Resource;
    }
}