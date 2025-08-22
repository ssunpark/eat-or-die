using System;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public enum EDragonPhase
{
    Phase1,
    Phase2,
}

public class DragonPhase
{
    private readonly DragonController _dragonController;
    public EDragonPhase CurrentPhase { get; private set; } = EDragonPhase.Phase1;

    private float _phaseThreshold;

    public DragonPhase(DragonController controller)
    {
        _dragonController = controller;
        _phaseThreshold = controller.ParamLoader.Base.PhaseThreshold;
        _dragonController.PhaseEffect.SetActive(false);
    }

    public bool EvaluatePhase(float hpRatio)
    {
        if (hpRatio <= _phaseThreshold)
        {
            SetPhase(EDragonPhase.Phase2);
            return true;
        }
        return false;
    }

    private void SetPhase(EDragonPhase phase)
    {
        CurrentPhase = phase;
        // 전환 시 연출 or 상태 변경 트리거
        _dragonController.PhaseEffect.SetActive(true);
        _dragonController.Animator.SetTrigger("Roar");
        _dragonController.Animator.SetBool("Awakening", true);
    }
    
    public void Death()
    {
        if(!_dragonController.HasStateAuthority) return;
        _dragonController.RPC_Death();
    }

    public void Dissolve()
    {
        _dragonController.Dissolve.PlayEffect();
        var quantity = Random.Range(5, 10);
        var dropPosition = _dragonController.transform.position;
        ItemProxySpawner.Instance.RPC_CreateItemObject(200028, quantity, 1f, dropPosition, quaternion.identity);
        _dragonController.IsDead = true;
    }
}