using Redcode.Pools;
using UnityEngine;

public class DragonMagicAttack_Lava : DragonSubStateBase
{
    private const string RAYCAST_MASK = "Floor";
    
    private DragonStateParameterSet.LavaParams _lavaParams;

    private int _spawnCount = 0;
    private float _nextSpawnTime;

    public Pool<Transform> _lavaPool;
    public Pool<Transform> _lavaFloorPool;

    public DragonMagicAttack_Lava(
        DragonController controller,
        IParentState parentState,
        DragonStateParameterSet.LavaParams lavaParams)
        : base(controller, parentState)
    {
        _lavaParams = lavaParams;
        
        GameObject lavaPool = new GameObject("LavaPool");
        _lavaPool = Pool.Create(_lavaParams.LavaPrefab.transform, 0, lavaPool.transform);
        _lavaFloorPool = Pool.Create(_lavaParams.LavaFloorPrefab.transform, 0, lavaPool.transform);
    }

    protected override void OnEnterState()
    {
        _spawnCount = 0;
        _nextSpawnTime = _lavaParams.StartDelay;

        Controller.Lock();
        Controller.Animator.SetBool("IsMove", false);
        Controller.Animator.SetTrigger("Attack_Lava");
    }

    protected override void OnFixedUpdate()
    {
        float t = Machine.StateTime;

        if (_spawnCount < _lavaParams.AngleList.Length && t >= _nextSpawnTime)
        {
            var lava = _lavaPool.Get();
            lava.transform.position = Controller.BreathPoint.position;

            // 각도/거리 계산
            float angle = _lavaParams.AngleList[_spawnCount];
            float distance = Random.Range(_lavaParams.MinDistance, _lavaParams.MaxDistance);

            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 direction = rot * Controller.transform.forward;
            Vector3 targetPosition = Controller.BreathPoint.position + direction * distance;

            Debug.DrawRay(targetPosition + Vector3.up * 5f, Vector3.down * 10f, Color.cyan, 10f);
            if (Physics.Raycast(targetPosition + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask(RAYCAST_MASK)))
            {
                targetPosition = hit.point;
            }

            lava.GetComponent<LavaProjectile>().Fire(
                new LavaProjectileData(targetPosition,
                    _lavaParams.LavaSpeed,
                    _lavaParams.FloorDuration,
                    _lavaParams.LavaHeight),
                () => _lavaPool.Take(lava),
                _lavaFloorPool
            );

            Debug.Log($"Lava {_spawnCount + 1} 생성 (각도 {angle}, 거리 {distance})");

            _spawnCount++;
            _nextSpawnTime = t + _lavaParams.Interval;
        }

        if (Controller.IsLocked)
        {
            return;
        }

        if (_spawnCount >= _lavaParams.AngleList.Length &&
            t >= _lavaParams.StartDelay + (_lavaParams.Interval * _lavaParams.AngleList.Length))
        {
            ParentState.OnSubStateComplete();
        }
    }
}