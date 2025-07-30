using System.Collections.Generic;
using Fusion.Addons.FSM;

public class DragonStateMachine
{
    public readonly DragonParameterLoader ParamLoader;
    public StateMachine<DragonStateBase> Machine { get; private set; }

    public DragonStateMachine(DragonController controller)
    {
        ParamLoader = new DragonParameterLoader();

        var idle = new DragonState_Idle(controller, ParamLoader);
        var alert = new DragonState_Alert(controller, ParamLoader);

        Machine = new StateMachine<DragonStateBase>("DragonStateMachine", idle, alert);
    }

    public void CollectStateMachines(List<IStateMachine> stateMachines)
    {
        stateMachines.Add(Machine);
    }
}