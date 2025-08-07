using Redcode.Pools;
using UnityEngine;

public class DragonMagicAttack_Lava : DragonSubStateBase
{
    private const string RAYCAST_MASK = "Floor";
    
    private DragonStateParameterSet.LavaParams _lavaParams;

    private int _spawnCount = 0;
    private float _nextSpawnTime;

    public DragonMagicAttack_Lava(
        DragonContext context,
        IParentState parentState)
        : base(context, parentState)
    {
        _lavaParams = Context.Parameter.Lava;
    }

    protected override void OnEnterState()
    {
        _spawnCount = 0;
        _nextSpawnTime = _lavaParams.StartDelay;

        Context.Movement.Lock();
        Context.Animator.SetBool("IsMove", false);
        Context.Animator.SetTrigger("Attack_Lava");
    }

    protected override void OnFixedUpdate()
    {
        if (!Context.Movement.IsLocked && 
            _spawnCount >= _lavaParams.AngleList.Length &&
            Machine.StateTime >= _lavaParams.StartDelay + (_lavaParams.Interval * _lavaParams.AngleList.Length))
        {
            ParentState.OnSubStateComplete();
        }
    }

    protected override void OnRender()
    {
        if (_spawnCount < _lavaParams.AngleList.Length && Machine.StateTime >= _nextSpawnTime)
        {
            float angle = _lavaParams.AngleList[_spawnCount];
            float distance = Random.Range(_lavaParams.MinDistance, _lavaParams.MaxDistance);

            var data = new LavaProjectileData(
                Vector3.zero, // targetPos는 Combat 내부에서 계산
                _lavaParams.LavaSpeed,
                _lavaParams.FloorDuration,
                _lavaParams.LavaHeight
            );

            Context.Combat.FireLava(
                Context.Transform.forward,
                angle,
                distance,
                data
            );

            Debug.Log($"Lava {_spawnCount + 1} 생성 (각도 {angle}, 거리 {distance})");

            _spawnCount++;
            _nextSpawnTime = Machine.StateTime + _lavaParams.Interval;
        }
    }
}