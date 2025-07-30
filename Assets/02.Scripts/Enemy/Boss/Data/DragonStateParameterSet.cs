using System;

[Serializable]
public class DragonStateParameterSet
{
    public BaseParams Base;
    public PatrolParams Patrol;
    public WaitParams Wait;
    public LookParams Look;

    [Serializable]
    public class BaseParams
    {
        public float MoveSpeed;
        public float RotationSpeed;
        public float HP;
        public float DetectRange;
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
    public class LookParams
    {
        public float LookDuration;
        public float AngleRange;
        public float MinAngleRange;
        public float WalkRange;
    }
}