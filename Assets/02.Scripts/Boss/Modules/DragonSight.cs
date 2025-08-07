using RaycastPro.Detectors;
using UnityEngine;

public class DragonSight
{
    private readonly DragonController _controller;
    
    public GameObject Target { get; private set; }
    public bool HasTarget => Target != null;
    
    public SightDetector SightDetector { get; private set; }
    
    public float Distance => Vector3.Distance(_controller.transform.position, Target.transform.position);

    public DragonSight(DragonController controller)
    {
        _controller = controller;
    }

    public void OnSpawned() { }

    public void SetSightDetector(float fullAwarenessRadius, float detectRadius, float detectAngle)
    {
        SightDetector = _controller.SightDetector;
        SightDetector.fullAwareness = fullAwarenessRadius;
        SightDetector.minRadius = fullAwarenessRadius;
        SightDetector.Radius = detectRadius;
        SightDetector.angleX = detectAngle;
    }

    public void SetTarget(GameObject target)
    {
        Target = target;
    }
}