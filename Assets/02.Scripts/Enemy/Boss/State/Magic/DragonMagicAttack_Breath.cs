using Redcode.Pools;
using UnityEngine;

public class DragonMagicAttack_Breath : DragonSubStateBase
{
    private DragonStateParameterSet.BreathParams _breathParams;
    private bool _hasFired;
    private bool _hasPlayedRenderEffect;

    public DragonMagicAttack_Breath(
        DragonController controller,
        IParentState parentState,
        DragonStateParameterSet.BreathParams breathParams)
        : base(controller, parentState)
    {
        _breathParams = breathParams;
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

        if (t >= _breathParams.FireTime + _breathParams.Duration)
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

            Vector3 spawnPos = Controller.BreathPoint.position;
            Quaternion rot = Quaternion.LookRotation(Controller.transform.forward);

            var localVfx = Controller.BreathParticlePool.Get();
            localVfx.transform.position = spawnPos;
            localVfx.transform.rotation = rot;
            localVfx.Init(_breathParams.Duration, ()=>Controller.BreathParticlePool.Take(localVfx));
        }
    }

    private void FireBreath()
    {
        if (!Controller.HasStateAuthority)
            return;

        Vector3 spawnPos = Controller.BreathPoint.position;
        Quaternion rot = Quaternion.LookRotation(Controller.transform.forward);

        var hitBoxObj = Controller.BreathHitBoxPool.Get();
        hitBoxObj.transform.position = spawnPos;
        hitBoxObj.transform.rotation = rot;
        hitBoxObj.Init(_breathParams.Duration, ()=>Controller.BreathHitBoxPool.Take(hitBoxObj));
    }
}