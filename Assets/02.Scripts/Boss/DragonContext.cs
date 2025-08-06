using UnityEngine;

public class DragonContext
{
    public DragonMovement Movement { get; private set; }
    public DragonCombat Combat { get; private set; }
    public DragonSight Sight { get; private set; }
    public DragonObjectPool Pool { get; private set; }
    public DragonParameterLoader Parameter { get; private set; }
    public Animator Animator { get; private set; }
    public Transform Transform { get; private set; }

    public DragonContext(DragonController controller)
    {
        Movement = new DragonMovement(controller);
        Combat = new DragonCombat(controller, this);
        Sight = new DragonSight(controller);
        Pool = new DragonObjectPool(controller);
        Parameter = new DragonParameterLoader();
        Animator = controller.GetComponent<Animator>();
        Transform = controller.transform;
    }

    public void OnSpawned()
    {
        Movement.OnSpawned();
        Combat.OnSpawned();
        Sight.OnSpawned();
    }
}