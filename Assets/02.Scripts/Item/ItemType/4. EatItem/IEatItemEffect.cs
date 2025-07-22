public interface IEatItemEffect
{
    public string Description { get; }
    // 타겟을 받도록 수정
    public void UseEffect();
}