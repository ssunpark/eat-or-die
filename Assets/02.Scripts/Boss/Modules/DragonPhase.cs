public enum EDragonPhase
{
    Phase1,
    Phase2,
}

public class DragonPhase
{
    public EDragonPhase CurrentPhase { get; private set; } = EDragonPhase.Phase1;

    private float _phaseThreshold;

    public DragonPhase(DragonStateParameterSet.BaseParams baseParams)
    {
        _phaseThreshold = baseParams.PhaseThreshold;
    }

    public void EvaluatePhase(float hpRatio)
    {
        if (hpRatio <= _phaseThreshold)
            SetPhase(EDragonPhase.Phase2);
    }

    private void SetPhase(EDragonPhase phase)
    {
        CurrentPhase = phase;
        // 전환 시 연출 or 상태 변경 트리거
    }
}