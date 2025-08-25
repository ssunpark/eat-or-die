using System.Collections;
using Fusion;
using UnityEngine;

public class DelayedNetworkSpawner : NetworkBehaviour
{
    [SerializeField]
    private bool _spawnWithStart = true;
    [SerializeField]
    private NetworkPrefabRef _prefab;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private float delaySeconds = 10f;

    public override void Spawned()
    {
        if (!_spawnWithStart)
            return;

        Spawn();
    }

    public void SpawnWithDelay()
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

        Spawn();
    }

    private void Spawn()
    {
        // 지정된 위치가 있으면 거기에, 없으면 자기 위치에 생성
        Vector3 pos = spawnPoint ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint ? spawnPoint.rotation : Quaternion.identity;

        var networkObject = Runner.Spawn(_prefab, pos, rot);
        if (networkObject.TryGetComponent(out IRespawnable spawnable))
        {
            spawnable.SetRespawnCallback(SpawnWithDelay);
        }
    }
}