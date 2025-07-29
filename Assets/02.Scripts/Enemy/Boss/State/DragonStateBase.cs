using Fusion.Addons.FSM;
using UnityEngine;

public class DragonStateBase : State<DragonStateBase>
{
    [HideInInspector]
    public DragonStateMachine StateMachine;

    public DragonStateBase(DragonStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }
}