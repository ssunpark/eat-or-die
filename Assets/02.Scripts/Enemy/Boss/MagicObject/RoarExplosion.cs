using System;
using System.Collections;
using Redcode.Pools;
using UnityEngine;

public class RoarExplosion : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField]
    private LavaFloor _lavaFloorPrefab;

    private float _radius;
    private int _spawnCount;
    private float _spawnInterval;
    private float _startAngle;

    private Coroutine _spawnRoutine;

    private Pool<LavaFloor> _lavaFloorPool;

    private void Awake()
    {
        GameObject explosion = new GameObject("ExplosionPool");
        _lavaFloorPool = Pool.Create(_lavaFloorPrefab, 0, explosion.transform);
    }

    /// <summary>
    /// 폭발 생성 시작 (360도 균등 배치)
    /// </summary>
    /// <param name="radius">반지름</param>
    /// <param name="count">생성할 개수</param>
    /// <param name="interval">생성 간 딜레이 (초)</param>
    public void Reset(float radius, int count, float interval)
    {
        _radius = radius;
        _spawnCount = count;
        _spawnInterval = interval;
        _startAngle = UnityEngine.Random.Range(0f, 360f);

        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
        }

        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        if (_lavaFloorPrefab == null || _spawnCount <= 0)
            yield break;

        float angleStep = 360f / _spawnCount;

        for (int i = 0; i < _spawnCount; i++)
        {
            float angleDeg = _startAngle - i * angleStep; // 시계 방향
            float angleRad = angleDeg * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad)) * _radius;
            Vector3 spawnPos = transform.position + offset;

            // 여기서 오브젝트를 생성합니다
            Debug.Log("폭발");
            var explosionObject = _lavaFloorPool.Get();
            explosionObject.transform.position = spawnPos;
            explosionObject.Lava.SetCallBack(() => _lavaFloorPool.Take(explosionObject));
            explosionObject.Init(3f);

#if UNITY_EDITOR
            Debug.DrawLine(transform.position, spawnPos, Color.red, 2f);
#endif

            yield return new WaitForSeconds(_spawnInterval);
        }

        _spawnRoutine = null;
    }
}