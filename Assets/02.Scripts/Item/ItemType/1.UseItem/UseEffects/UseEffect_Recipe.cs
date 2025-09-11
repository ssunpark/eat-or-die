using UnityEngine;

public class UseEffect_Recipe : IUseEffect
{
    public void Use(GameObject target)
    {
        // 레시피 해금
        Debug.Log("UseEffect_Recipe");
    }
}