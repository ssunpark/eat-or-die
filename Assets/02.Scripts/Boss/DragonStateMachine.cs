using System.Collections.Generic;
using Fusion.Addons.FSM;

public class DragonStateMachine
{
    public StateMachine<DragonStateBase> Machine { get; private set; }

    public DragonStateMachine(DragonContext context)
    {
        var idle = new DragonState_Idle(context);
        var alert = new DragonState_Alert(context);
        var attack = new DragonState_MeleeAttack(context);
        var chase = new DragonState_Chase(context);
        var magic = new DragonState_MagicAttack(context);

        Machine = new StateMachine<DragonStateBase>("DragonStateMachine", idle, alert, attack, chase, magic);
    }

    public void CollectStateMachines(List<IStateMachine> stateMachines)
    {
        stateMachines.Add(Machine);
    }
}