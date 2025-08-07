using UnityEngine;

public class EnemyAnimationRelay : MonoBehaviour
{
    private IEventReceiver _eventReceiver;

    public void SetReceiver(IEventReceiver eventReceiver)
    {
        _eventReceiver = eventReceiver;
    }
    
    public void RemoveReceiver()
    {
        _eventReceiver = null;
    }

    public void CallAnimationEvent()
    {
        _eventReceiver?.OnActionMoment();
    }
}