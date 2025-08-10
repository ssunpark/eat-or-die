using UnityEngine;

public class EnemyAnimationRelay : MonoBehaviour
{
    private EnemyBehaviourMachine _machine;
    
    public void SetMachine(EnemyBehaviourMachine machine)
    {
        _machine = machine;
    }

    public void CallAnimationEvent()
    {
        if (_machine.ActiveState is IEventReceiver eventReceiver)
        {
            eventReceiver.OnActionMoment();
        }
    }
}