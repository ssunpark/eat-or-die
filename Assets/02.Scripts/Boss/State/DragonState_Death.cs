public class DragonState_Death : DragonStateBase, IAnimationActionNotify
{
    public DragonState_Death(DragonContext context) : base(context)
    {
    }

    protected override void OnEnterStateRender()
    {
        // 죽는 애니메이션
        Context.Phase.Death();
    }

    public void OnActionMoment()
    {
        // 디졸브 & 아이템 드랍
        Context.Phase.Dissolve();
    }
}