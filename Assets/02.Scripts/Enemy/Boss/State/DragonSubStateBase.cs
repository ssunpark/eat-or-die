using Fusion.Addons.FSM;
using UnityEngine;

public class DragonSubStateBase : State<DragonSubStateBase>
{
    [HideInInspector]
    public DragonController Controller;

    [HideInInspector]
    public IParentState ParentState;

    public DragonSubStateBase(DragonController controller, IParentState parentState)
    {
        Controller = controller;
        ParentState = parentState;
    }
}