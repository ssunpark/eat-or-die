using System.Collections;
using Fusion;
using UnityEngine;

public class DelayedNetworkSpawner : NetworkBehaviour
{
    [SerializeField]
    private NetworkPrefabRef _prefab;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private float delaySeconds = 10f;

    public void Spawn()
    {
        if (!Runner.IsServer)
        {
            return;
        }
        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        // delaySeconds 만큼 기다림
        yield return new WaitForSeconds(delaySeconds);

        // 지정된 위치가 있으면 거기에, 없으면 자기 위치에 생성
        Vector3 pos = spawnPoint ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint ? spawnPoint.rotation : Quaternion.identity;

        Runner.Spawn(_prefab, pos, rot);
    }
}