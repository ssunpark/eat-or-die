using DarkTonic.MasterAudio;
using UnityEngine;

public class UI_NpcTalkSound : MonoBehaviour
{
    [SerializeField] private Transform _npc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MasterAudio.PlaySound3DAtTransform("NpcTalk", _npc.transform);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MasterAudio.StopSoundGroupOfTransform(_npc.transform, "NpcTalk");
        }
    }
}