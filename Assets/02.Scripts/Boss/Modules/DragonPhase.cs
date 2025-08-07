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

    public DragonPhase(DragonController controller, DragonStateParameterSet.BaseParams baseParams)
    {
        _dragonController = controller;
        _phaseThreshold = baseParams.PhaseThreshold;
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
    }
}