using System;
using UnityEngine;

[Serializable]
public class DragonStateParameterSet
{
    // 기본 데이터
    public BaseParams Base;
    // - 배회 데이터
    public PatrolParams Patrol;
    // - 대기 데이터
    public WaitParams Wait;
    // 경계 데이터
    public AlertParams Alert;
    // 근거리 공격 데이터
    public AttackParams Attack;
    // - 근거리 공격 시 준비
    public PrepareParams Prepare;
    // - 근거리 공격 패턴
    public SwipeParams Swipe;
    public RightScratchParams RightScratch;
    public LeftScratchParams LeftScratch;
    public BiteParams Bite;
    // 추격 상태
    public ChaseParams Chase;
    // 마법 공격 데이터
    public MagicParams Magic;
    public BreathParams Breath;

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
    public class ChaseParams
    {
        public float ChaseSpeed;
        public float RotationSpeed;
        public float SidestepProbability;
        public float MinSidestepAngle;
        public float MaxSidestepAngle;
        public float SidestepRange;
        public float MinSidestepDistance;
    }

    [Serializable]
    public class AttackParams
    {
        public float ContinueAttackChance;
    }

    [Serializable]
    public class PrepareParams
    {
        public float PrepareDuration;
        public float MinDistanceToFinishPrepare;
        public float PrepareChance;
    }

    [Serializable]
    public class SwipeParams
    {
        // 공격 범위 관련 데이터
    }

    [Serializable]
    public class RightScratchParams
    {
        // 공격 범위 관련 데이터
    }

    [Serializable]
    public class LeftScratchParams
    {
        // 공격 범위 관련 데이터
    }

    [Serializable]
    public class BiteParams
    {
    }
    
    [Serializable]
    public class MagicParams
    {
        public float ContinueMagicProbability;
    }
    
    [Serializable]
    public class BreathParams
    {
        public float FireTime;       // 발사 타이밍
        public float TotalDuration;  // 전체 지속 시간
        public string BreathAddress; // 어드레서블 키
        
        [NonSerialized]
        public GameObject BreathPrefab;
    }
}