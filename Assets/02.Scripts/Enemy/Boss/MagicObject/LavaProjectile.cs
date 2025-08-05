using System;
using DG.Tweening;
using Redcode.Pools;
using UnityEngine;

public class LavaProjectile : MonoBehaviour
{
    private Vector3 _targetPosition;
    private float _duration;
    private float _height;

    private Tween _moveTween;

    private Pool<Transform> _lavaFloorPool;

    private Action _onDespawnCallback;

    public void Fire(Vector3 targetPosition, float speed, float duration, float height, Action OnDespawnCallback,
        Pool<Transform> floorPool)
    {
        _targetPosition = targetPosition;
        _height = height;
        _duration = duration;
        _lavaFloorPool = floorPool;
        _onDespawnCallback = OnDespawnCallback;

        StartParabolaMove(speed);
    }

    private void StartParabolaMove(float speed)
    {
        Vector3 startPos = transform.position;

        // 중간 정점 계산
        Vector3 midPoint = (startPos + _targetPosition) / 2f;
        midPoint.y += _height;

        // 전체 경로
        Vector3[] path = new Vector3[] { midPoint, _targetPosition };

        // 거리 계산 (CatmullRom은 곡선이라 근사)
        float totalDistance = Vector3.Distance(startPos, midPoint) + Vector3.Distance(midPoint, _targetPosition);
        float duration = totalDistance / speed;

        _moveTween?.Kill();

        _moveTween = transform
            .DOPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(OnArrived);
    }

    private void OnArrived()
    {
        Debug.Log("도착 완료!");
        // 이펙트나 데미지 처리 등 추가 가능
        var floor = _lavaFloorPool.Get();
        floor.position = _targetPosition;
        floor.GetComponent<LavaFloor>().Init(_duration, ()=>_lavaFloorPool.Take(floor));
        _onDespawnCallback?.Invoke();
    }
}