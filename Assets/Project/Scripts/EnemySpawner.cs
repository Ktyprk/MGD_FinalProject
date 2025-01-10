using Photon.Pun;
using UnityEngine;
using System.Collections;
using System.IO;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints; 

    private bool shouldSpawn = false;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(WaitAndStartSpawning(2f));
        }
    }

    IEnumerator WaitAndStartSpawning(float delay)
    {
        yield return new WaitForSeconds(delay);
        shouldSpawn = true;
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (shouldSpawn)
        {
            float spawnDelay = Random.Range(5f, 10f); 
            yield return new WaitForSeconds(spawnDelay);

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            int enemyType = Random.Range(0, 2);

            if (enemyType == 0)
            {
                PhotonNetwork.Instantiate(
                    Path.Combine("PhotonPrefabs", "BatEnemy"),
                    spawnPoint.position, spawnPoint.rotation
                );
            }
            else
            {
                PhotonNetwork.Instantiate(
                    Path.Combine("PhotonPrefabs", "GhostEnemy"),
                    spawnPoint.position, spawnPoint.rotation
                );
            }
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient && !shouldSpawn)
        {
            StartCoroutine(WaitAndStartSpawning(2f));
        }
    }
}