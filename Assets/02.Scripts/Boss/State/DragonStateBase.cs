using Fusion.Addons.FSM;
using UnityEngine;

public class DragonStateBase : State<DragonStateBase>
{
    public DragonContext Context { get; private set; }

    public DragonStateBase(DragonContext context)
    {
        Context = context;
    }
}