using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyContext
{
    public GameObject Target;
    public EnemyStatManager StatManager;
    public Animator Animator;
    public EnemyAnimationRelay AnimationRelay;
    public NavMeshAgent Agent;
    public IMoveable Mover;
    public IDetector Detector;
}