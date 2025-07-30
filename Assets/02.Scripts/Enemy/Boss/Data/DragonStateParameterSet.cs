using System;

[Serializable]
public class DragonStateParameterSet
{
    public BaseParams Base;
    public PatrolParams Patrol;
    public WaitParams Wait;
    public AlertParams Alert;
    public PrepareParams Prepare;
    public SwipeParams Swipe;

    [Serializable]
    public class BaseParams
    {
        public float MoveSpeed;
        public float RotationSpeed;
        public float HP;
        public float DetectRadius;
        public float FullAwarenessRadius;
        public float FOVAngle;
        public float AnimSmoothSpeed;
        public float MeleeAttackDistance;
    }

    [Serializable]
    public class PatrolParams
    {
        public float PatrolDuration;
        public float WalkRadius;
        public float MinWalkRadius;
    }

    [Serializable]
    public class WaitParams
    {
        public float WaitDuration;
    }
    
    [Serializable]
    public class AlertParams
    {
        public float LookProbability => 1.0f - ChaseProbability - RangedProbability;
        public float ChaseProbability;
        public float RangedProbability;
        public float LookDuration;
        public float AngleRange;
        public float MinAngleRange;
        public float WalkRange;
        public float MinDistance;
    }
    
    [Serializable]
    public class PrepareParams
    {
        public float PrepareDuration;
    }

    [Serializable]
    public class SwipeParams
    {
        public float SwipeDuration;
    }
}