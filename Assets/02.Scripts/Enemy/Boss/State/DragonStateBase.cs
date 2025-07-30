using Fusion.Addons.FSM;
using UnityEngine;

public class DragonStateBase : State<DragonStateBase>
{
    [HideInInspector]
    public DragonController Controller;
    
    [HideInInspector]
    public DragonParameterLoader ParameterLoader;

    public DragonStateBase(DragonController controller, DragonParameterLoader parameterLoader)
    {
        Controller = controller;
        ParameterLoader = parameterLoader;
    }
}