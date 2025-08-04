using Redcode.Pools;
using UnityEngine;

public class DragonMagicAttack_Breath : DragonSubStateBase
{
    private DragonStateParameterSet.BreathParams _breathParams;
    private bool _hasFired;
    private bool _hasPlayedRenderEffect;

    private Pool<Transform> _hitTargetPool;
    private Pool<Transform> _particlePool;

    public DragonMagicAttack_Breath(
        DragonController controller,
        IParentState parentState,
        DragonStateParameterSet.BreathParams breathParams)
        : base(controller, parentState)
    {
        _breathParams = breathParams;
        _hitTargetPool = Pool.Create(_breathParams.BreathHitboxPrefab.transform, 3, Controller.transform).NonLazy();
        _particlePool = Pool.Create(_breathParams.LocalBreathParticle.transform, 3, Controller.transform).NonLazy();
    }

    protected override bool CanEnterState()
    {
        return _breathParams.BreathHitboxPrefab != null;
    }

    protected override void OnEnterState()
    {
        _hasFired = false;

        Controller.Lock();
        Controller.Animator.SetBool("IsMove", false);
        Controller.Animator.SetBool("Attack_Breath", true);
    }

    protected override void OnFixedUpdate()
    {
        float t = Machine.StateTime;

        if (!_hasFired && t >= _breathParams.FireTime)
        {
            FireBreath();
            _hasFired = true;
        }

        if (t >= _breathParams.FireTime + _breathParams.TotalDuration)
        {
            Controller.Animator.SetBool("Attack_Breath", false);
        }

        if (!Controller.IsLocked)
        {
            ParentState.OnSubStateComplete();
        }
    }

    protected override void OnEnterStateRender()
    {
        _hasPlayedRenderEffect = false;
    }

    protected override void OnRender()
    {
        float t = Machine.StateTime;

        if (!_hasPlayedRenderEffect && t >= _breathParams.FireTime)
        {
            _hasPlayedRenderEffect = true;

            if (_breathParams.LocalBreathParticle == null)
                return;

            Vector3 spawnPos = Controller.BreathPoint.position;
            Quaternion rot = Quaternion.LookRotation(Controller.transform.forward);

            var localVfx = _particlePool.Get();
            localVfx.transform.position = spawnPos;
            localVfx.transform.rotation = rot;
            localVfx.GetComponent<BreathParticle>()?.Init(_breathParams.TotalDuration, ()=>_particlePool.Take(localVfx));
        }
    }

    private void FireBreath()
    {
        if (!Controller.HasStateAuthority)
            return;

        Vector3 spawnPos = Controller.BreathPoint.position;
        Quaternion rot = Quaternion.LookRotation(Controller.transform.forward);

        var hitBoxObj = _hitTargetPool.Get();
        hitBoxObj.transform.position = spawnPos;
        hitBoxObj.transform.rotation = rot;
        hitBoxObj.GetComponent<DragonBreathHitBox>()?.Init(_breathParams.TotalDuration, ()=>_hitTargetPool.Take(hitBoxObj));
    }
}