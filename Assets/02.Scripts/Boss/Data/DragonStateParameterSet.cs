using System;
using UnityEngine;

[Serializable]
public class DragonStateParameterSet
{
    // 기본 공통 파라미터 (모든 상태 공용)
    public BaseParams Base;

    // 배회(Patrol) 상태 관련 파라미터
    public PatrolParams Patrol;

    // 대기(Wait) 상태 관련 파라미터
    public WaitParams Wait;

    // 경계(Alert) 상태 관련 파라미터
    public AlertParams Alert;

    // 근거리 공격 공통 파라미터
    public AttackParams Attack;

    // 근거리 공격 전 준비 상태 파라미터
    public PrepareParams Prepare;

    // 근거리 공격 패턴들
    public SwipeParams Swipe;
    public RightScratchParams RightScratch;
    public LeftScratchParams LeftScratch;
    public BiteParams Bite;

    // 추격 상태 파라미터
    public ChaseParams Chase;

    // 마법 공격 공통 파라미터
    public MagicParams Magic;

    // 브레스 공격 관련 파라미터
    public BreathParams Breath;

    // Lava 공격 관련 파라미터
    public LavaParams Lava;

    // Roar 공격 관련 파라미터
    public RoarParams Roar;

    [Serializable]
    public class BaseParams
    {
        public float MoveSpeed;           // 이동 속도
        public float RotationSpeed;       // 회전 속도
        public float HP;                  // 체력
        public float PhaseThreshold;
        public float DetectRadius;        // 기본 감지 범위 (FOV 포함)
        public float FullAwarenessRadius; // 전방향 감지 거리
        public float FOVAngle;            // 시야각
        public float AnimSmoothSpeed;     // 애니메이션 블렌딩 속도
        public float MeleeAttackDistance; // 근접 공격 사정거리
    }

    [Serializable]
    public class PatrolParams
    {
        public float PatrolDuration; // 배회 지속 시간
        public float WalkRadius;     // 최대 이동 반경
        public float MinWalkRadius;  // 최소 이동 반경
    }

    [Serializable]
    public class WaitParams
    {
        public float WaitDuration; // 대기 시간
    }

    [Serializable]
    public class AlertParams
    {
        public float ChaseProbability; // 추격 상태로 전이 확률
        public float MagicProbability; // 원거리 공격 상태로 전이 확률
        public float LookDuration;     // 경계 행동 시간
        public float AngleRange;       // 최대 시선 회전 각도
        public float MinAngleRange;    // 최소 시선 회전 각도
        public float WalkRange;        // 시선 위치 이동 거리 범위
        public float MinDistance;      // 최소 이동 거리 (너무 가까운 목적지 방지)
    }

    [Serializable]
    public class ChaseParams
    {
        public float ChaseSpeed;          // 추격 속도
        public float RotationSpeed;       // 추격 중 회전 속도
        public float SidestepProbability; // 사이드스텝 시도 확률
        public float MinSidestepAngle;    // 최소 회피 각도
        public float MaxSidestepAngle;    // 최대 회피 각도
        public float SidestepRange;       // 회피 거리 범위
        public float MinSidestepDistance; // 최소 회피 거리
    }

    [Serializable]
    public class AttackParams
    {
        public float ContinueAttackChance; // 연속 공격 시도 확률
    }

    [Serializable]
    public class PrepareParams
    {
        public float PrepareDuration;            // 준비 상태 지속 시간
        public float MinDistanceToFinishPrepare; // 준비 종료를 위한 최소 거리
        public float PrepareChance;              // 준비 상태 진입 확률
    }

    [Serializable]
    public class SwipeParams
    {
        // 스와이프 공격 전용 파라미터 (추후 구현 필요)
    }

    [Serializable]
    public class RightScratchParams
    {
        // 오른손 할퀴기 공격 파라미터 (추후 구현 필요)
    }

    [Serializable]
    public class LeftScratchParams
    {
        // 왼손 할퀴기 공격 파라미터 (추후 구현 필요)
    }

    [Serializable]
    public class BiteParams
    {
        // 물기 공격 파라미터 (추후 구현 필요)
    }

    [Serializable]
    public class MagicParams
    {
        public float ContinueMagicProbability; // 연속 마법 사용 확률
        public float NearMagicProbability;     // 근접 마법 사용 확률
    }

    [Serializable]
    public class BreathParams
    {
        public float FireTime; // 브레스 시작 시점 (예: 준비 동작 후)
        public float Duration; // 브레스 지속 시간
    }

    [Serializable]
    public class LavaParams
    {
        public float StartDelay;    // 애니메이션 시작 후 딜레이
        public float Interval;      // 발사 인터벌
        public float MinDistance;   // 최소 거리
        public float MaxDistance;   // 최대 거리
        public float LavaSpeed;     // 투사체 이동 속도
        public float LavaHeight;    // 투사체 포물선 높이
        public float FloorDuration; // 바닥 지속 시간

        public float[] AngleList;
    }

    [Serializable]
    public class RoarParams
    {
        public float FireTime; // 기술 시작 시점 (예: 준비 동작 후)
        public float Duration; // 기술 전체 지속 시간
        public float Radius;   // 기술 반경
        public int Count;      // 폭발 생성 갯수
    }
}