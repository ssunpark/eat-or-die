using UnityEngine;

public class EnemyAnimationRelay : MonoBehaviour
{
    private EnemyBehaviourMachine _machine;
    
    public void SetMachine(EnemyBehaviourMachine machine)
    {
        _machine = machine;
    }

    public void PlayParticleEvent()
    {
        if (_machine.ActiveState is IParticlePlayer particlePlayer)
        {
            particlePlayer.PlayParticle();
        }
    }

    public void CallAnimationEvent()
    {
        if (_machine.ActiveState is IEventReceiver eventReceiver)
        {
            eventReceiver.OnActionMoment();
        }
    }
}