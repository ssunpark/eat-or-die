using System;
using UnityEngine;
//수현
public class CookingPotInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("요리 슬롯 인터페이스 활성화");
        }
    }
}
