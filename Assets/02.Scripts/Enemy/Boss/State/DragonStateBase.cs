using Fusion.Addons.FSM;
using UnityEngine;

public class DragonStateBase : State<DragonStateBase>
{
    [HideInInspector]
    public DragonController Controller;

    public DragonStateBase(DragonController controller)
    {
        Controller = controller;
    }
}