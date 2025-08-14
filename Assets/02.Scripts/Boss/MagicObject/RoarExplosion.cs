// RoarExplosion.cs (핵심만)

using System.Collections;
using Fusion;
using UnityEngine;

public class RoarExplosion : NetworkBehaviour
{
    [SerializeField]
    private LavaFloor _lavaFloorPrefab;

    private float _radius;
    private float _spawnInterval;
    private int _spawnCount;
    private float _startAngle;
    private Coroutine _spawnRoutine;

    private float _floorDamage;

    public void Reset(float radius, int count, float interval, float floorDamage)
    {
        if (!HasStateAuthority)
            return; // 권위만 실행

        _radius = radius;
        _spawnCount = count;
        _spawnInterval = interval;
        _startAngle = Random.Range(0f, 360f);
        _floorDamage = floorDamage;

        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);

        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        float step = 360f / _spawnCount;

        for (int i = 0; i < _spawnCount; i++)
        {
            float ang = _startAngle - i * step;
            Vector3 offset = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), 0f, Mathf.Sin(ang * Mathf.Deg2Rad)) * _radius;
            Vector3 pos = transform.position + offset;

            var floor = Runner.Spawn(_lavaFloorPrefab, pos, Quaternion.identity,
                onBeforeSpawned: (runner, obj) =>
                {
                    var floor = obj.GetComponent<LavaFloor>();
                    floor.StartPosition = pos;
                    floor.StartTick = Runner.Tick;
                    floor.Duration = _spawnInterval * 2f;
                    floor.SetDamage(_floorDamage);
                });

            yield return new WaitForSeconds(_spawnInterval);
        }

        _spawnRoutine = null;
    }
}