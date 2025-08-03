using UnityEngine;

public class DragonMagicAttack_Breath : DragonSubStateBase
{
    private DragonStateParameterSet.BreathParams _breathParams;
    private bool _hasFired;

    public DragonMagicAttack_Breath(
        DragonController controller,
        IParentState parentState,
        DragonStateParameterSet.BreathParams breathParams)
        : base(controller, parentState)
    {
        _breathParams = breathParams;
    }

    protected override bool CanEnterState()
    {
        return _breathParams.BreathPrefab != null;
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

    private void FireBreath()
    {
        Vector3 spawnPos = Controller.BreathPoint.position;
        Quaternion rot = Quaternion.LookRotation(Controller.transform.forward);

        Machine.Runner.Spawn(
            prefab: _breathParams.BreathPrefab,
            position: spawnPos,
            rotation: rot,
            onBeforeSpawned: (runner, obj) =>
            {
                obj.GetComponent<DragonBreath>().Init(_breathParams.TotalDuration);
            }
        );
    }
}