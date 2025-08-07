using UnityEngine;

public class DragonMagicAttack_Lava : DragonSubStateBase, IAnimationActionNotify, IAnimationExitActionNotify
{
    private const string RAYCAST_MASK = "Floor";
    
    private DragonStateParameterSet.LavaParams _lavaParams;

    private int _spawnCount = 0;

    public DragonMagicAttack_Lava(
        DragonContext context,
        IParentState parentState)
        : base(context, parentState)
    {
        _lavaParams = Context.Parameter.Lava;
    }

    protected override void OnEnterState()
    {
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

    protected override void OnEnterStateRender()
    {
        _spawnCount = 0;
    }

    public void OnActionMoment()
    {
        if (_spawnCount < _lavaParams.AngleList.Length)
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
        }
    }

    public void OnExitMoment()
    {
        Context.Movement.Unlock();
    }
}