using DG.Tweening;
using UnityEngine;

public class DragonMagicAttack_Roar : DragonSubStateBase
{
    private DragonStateParameterSet.RoarParams _roarParams;
    private bool _onFired;

    private Sequence effectsSequence;

    public DragonMagicAttack_Roar(
        DragonController controller,
        IParentState parentState,
        DragonStateParameterSet.RoarParams roarParams)
        : base(controller, parentState)
    {
        _roarParams = roarParams;

        effectsSequence = DOTween.Sequence();
        effectsSequence.SetAutoKill(false)
            .AppendCallback(() =>
            {
                Controller.RoarEffect.SetActive(true);
            })
            .Append(Controller.RoarEffect.transform.DOScale(Vector3.zero, 0f))
            .Append(Controller.RoarEffect.transform.DOScale(Vector3.one * 0.8f, _roarParams.Duration / 4f))
            .AppendInterval(_roarParams.Duration / 2f)
            .Append(Controller.RoarEffect.transform.DOScale(Vector3.zero, _roarParams.Duration / 4f))
            .AppendCallback(() => Controller.RoarEffect.SetActive(false));
        effectsSequence.Pause();
    }

    protected override void OnEnterState()
    {
        Controller.Lock();
        Controller.Animator.SetBool("IsMove", false);
        Controller.Animator.SetBool("Attack_Roar", true);
    }

    protected override void OnFixedUpdate()
    {
        if (!_onFired && Machine.StateTime >= _roarParams.FireTime)
        {
            _onFired = true;
            // 발사
            float interval = _roarParams.Duration / _roarParams.Count;
            Controller.RoarExplosion.Reset(_roarParams.Radius, _roarParams.Count, interval);
            effectsSequence.Restart();
            return;
        }

        if (Machine.StateTime >= _roarParams.FireTime + _roarParams.Duration)
        {
            Controller.Animator.SetBool("Attack_Roar", false);
        }

        if (!Controller.IsLocked)
        {
            ParentState.OnSubStateComplete();
        }

        return;
    }

    protected override void OnExitState()
    {
        _onFired = false;
    }
}