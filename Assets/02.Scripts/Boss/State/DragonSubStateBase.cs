using Fusion.Addons.FSM;
using UnityEngine;

public class DragonSubStateBase : State<DragonSubStateBase>
{
    public DragonContext Context { get; private set; }
    public IParentState ParentState { get; private set; }

    public DragonSubStateBase(DragonContext context, IParentState parentState)
    {
        Context = context;
        ParentState = parentState;
    }
}