using UnityEngine;

public class DragonAnimation
{
    private readonly DragonController _controller;

    public DragonAnimation(DragonController controller)
    {
        _controller = controller;
    }

    public void SetRandomWaitAnimation()
    {
        if (!_controller.HasStateAuthority)
        {
            return;
        }
        
        _controller.AnimWaitIndex = Random.Range(0, 2);
    }
}