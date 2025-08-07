using UnityEngine;

public class DragonContext
{
    public DragonMovement Movement { get; private set; }
    public DragonCombat Combat { get; private set; }
    public DragonSight Sight { get; private set; }
    public DragonParameterLoader Parameter { get; private set; }
    public DragonStats Stats { get; private set; }
    public DragonPhase Phase { get; private set; }
    public Animator Animator { get; private set; }
    public Transform Transform { get; private set; }

    public DragonContext(DragonController controller)
    {
        Parameter = controller.ParamLoader;
        Animator = controller.GetComponent<Animator>();
        Transform = controller.transform;
        
        Movement = new DragonMovement(controller);
        Combat = new DragonCombat(controller);
        Sight = new DragonSight(controller);
        Phase = new DragonPhase(controller);
        Stats = new DragonStats(controller);
    }

    public void OnSpawned()
    {
        Movement.OnSpawned();
        Combat.OnSpawned();
        Sight.OnSpawned();
    }
}