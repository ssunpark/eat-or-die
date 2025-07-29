using Fusion.Addons.FSM;
using UnityEngine;

public class DragonSubStateBase : State<DragonSubStateBase>
{
    [HideInInspector]
    public DragonStateMachine StateMachine;

    [HideInInspector]
    public IParentStateMachine ParentStateMachine;

    public DragonSubStateBase(DragonStateMachine stateMachine, IParentStateMachine parentStateMachine)
    {
        StateMachine = stateMachine;
        ParentStateMachine = parentStateMachine;
    }
}