using System;
using DG.Tweening;
using Redcode.Pools;
using UnityEngine;

public struct LavaProjectileData
{
    public readonly Vector3 TargetPosition;
    public readonly float Speed;
    public readonly float Duration;
    public readonly float Height;

    public LavaProjectileData(Vector3 targetPosition, float speed, float duration, float height)
    {
        TargetPosition = targetPosition;
        Speed = speed;
        Duration = duration;
        Height = height;
    }
}

public class LavaProjectile : MonoBehaviour
{
    private LavaProjectileData _lavaProjectileData;

    private Tween _moveTween;

    private Pool<LavaFloor> _lavaFloorPool;

    private Action _onDespawnCallback;

    public void Fire(LavaProjectileData projectileData, Action OnDespawnCallback,
        Pool<LavaFloor> floorPool)
    {
        _lavaProjectileData = projectileData;
        _lavaFloorPool = floorPool;
        _onDespawnCallback = OnDespawnCallback;

        StartParabolaMove();
    }

    private void StartParabolaMove()
    {
        Vector3 startPos = transform.position;

        // 중간 정점 계산
        Vector3 midPoint = (startPos + _lavaProjectileData.TargetPosition) / 2f;
        midPoint.y += _lavaProjectileData.Height;

        // 전체 경로
        Vector3[] path = new Vector3[] { midPoint, _lavaProjectileData.TargetPosition };

        // 거리 계산 (CatmullRom은 곡선이라 근사)
        float totalDistance =
            Vector3.Distance(startPos, midPoint) + Vector3.Distance(midPoint, _lavaProjectileData.TargetPosition);
        float duration = totalDistance / _lavaProjectileData.Speed;

        _moveTween?.Kill();

        _moveTween = transform
            .DOPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(OnArrived);
    }

    private void OnArrived()
    {
        // 이펙트나 데미지 처리 등 추가 가능
        var floor = _lavaFloorPool.Get();
        floor.transform.position = _lavaProjectileData.TargetPosition;
        floor.Effect.SetCallBack(() => _lavaFloorPool.Take(floor));
        floor.Init(_lavaProjectileData.Duration);

        _onDespawnCallback?.Invoke();
    }
}