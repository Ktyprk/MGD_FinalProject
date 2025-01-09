using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy1; // İlk düşman tipi
    public GameObject enemy2; // İkinci düşman tipi
    public Transform spawnPoint; // Spawn olma noktası
    public float spawnRadius = 30f; // Minimum spawn mesafesi
    public float spawnOuterRadius = 45f; // Maksimum spawn mesafesi

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            float spawnDelay = Random.Range(2f, 5f); // 2 ila 5 saniye arası rastgele süre
            yield return new WaitForSeconds(spawnDelay);

            Vector3 randomOffset = Random.insideUnitSphere * spawnOuterRadius;
            randomOffset.y = 0; 

            if (randomOffset.magnitude < spawnRadius)
            {
                randomOffset = randomOffset.normalized * spawnRadius;
            }

            Vector3 spawnPosition = spawnPoint.position + randomOffset;

            int enemyType = Random.Range(0, 2); // 0 veya 1

            if (enemyType == 0)
            {
                enemy1.GetComponent<EnemyMovement>().target = this.gameObject;
                Instantiate(enemy1, spawnPosition, spawnPoint.rotation);
            }
            else
            {
                enemy2.GetComponent<EnemyMovement>().target = this.gameObject;
                Instantiate(enemy2, spawnPosition, spawnPoint.rotation);
            }
        }
    }
}