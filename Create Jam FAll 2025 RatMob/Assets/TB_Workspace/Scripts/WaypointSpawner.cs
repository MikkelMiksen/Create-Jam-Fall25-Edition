using UnityEngine;

public class WaypointSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private int spawnCount = 10;

    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnPrefab();
        }
    }

    void SpawnPrefab()
    {
        float randomX = Random.Range(-transform.localScale.x*10, transform.localScale.x*10);
        float randomZ = Random.Range(-transform.localScale.z*10, transform.localScale.z*10);
        Vector3 spawnPos = new Vector3(randomX, 1f, randomZ);

        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }
}
