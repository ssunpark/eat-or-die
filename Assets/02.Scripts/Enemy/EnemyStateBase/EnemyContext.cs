using UnityEngine;
using UnityEngine.AI;

public class EnemyContext
{
    public Transform Target;
    public EnemyStat Stat;
    public Animator Animator;
    public NavMeshAgent Agent;
    public IMoveable Mover;
}