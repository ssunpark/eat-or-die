using System.Collections.Generic;
using Fusion.Addons.FSM;

public class DragonStateMachine
{
    public readonly DragonParameterLoader ParamLoader;
    public StateMachine<DragonStateBase> Machine { get; private set; }

    public DragonStateMachine(DragonController controller)
    {
        ParamLoader = new DragonParameterLoader();

        ParamLoader.LoadAddressablesAsync();

        var idle = new DragonState_Idle(controller, ParamLoader);
        var alert = new DragonState_Alert(controller, ParamLoader);
        var attack = new DragonState_MeleeAttack(controller, ParamLoader);
        var chase = new DragonState_Chase(controller, ParamLoader);
        var magic = new DragonState_MagicAttack(controller, ParamLoader);

        Machine = new StateMachine<DragonStateBase>("DragonStateMachine", idle, alert, attack, chase, magic);
    }

    public void CollectStateMachines(List<IStateMachine> stateMachines)
    {
        stateMachines.Add(Machine);
    }
}