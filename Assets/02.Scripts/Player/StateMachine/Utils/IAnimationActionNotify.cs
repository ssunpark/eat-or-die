public interface IAnimationActionNotify
{
    /// <summary>
    /// 애니메이션 액션이 발생하는 순간에 호출됩니다.
    /// ex. 공격 애니메이션 중 적에게 피해를 주는 순간
    /// ex2. 상호작용 애니메이션 중 이펙트를 발생시키는 순간
    /// </summary>
    void OnActionMoment();
}