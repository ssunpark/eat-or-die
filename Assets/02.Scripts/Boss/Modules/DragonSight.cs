using RaycastPro.Detectors;
using UnityEngine;

public class DragonSight
{
    private readonly DragonController _controller;

    public GameObject Target => _controller.TargetPlayer.gameObject;
    public bool HasTarget => _controller.TargetPlayer?.gameObject != null;

    public SightDetector SightDetector { get; private set; }

    public float Distance
    {
        get
        {
            if (!HasTarget)
            {
                return float.MaxValue;
            }
            return Vector3.Distance(_controller.transform.position, _controller.TargetPlayer.transform.position);
        }
    }

    public DragonSight(DragonController controller)
    {
        _controller = controller;
    }

    public void OnSpawned()
    {
        if (!_controller.HasStateAuthority)
        {
            SightDetector.enabled = false;
        }
    }

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
        _controller.SetTarget(target);
    }
}