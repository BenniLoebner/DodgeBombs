using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject enemy; // The object to spawn
    public GameObject points; // The points prefab to instantiate

    public float minEnemySpawnInterval = 2f; // Time interval between spawns
    public float maxEnemySpawnInterval = 5f; // Time interval between spawns

    public float minPointsSpawnInterval = 2f; // Time interval between spawns
    public float maxPointsSpawnInterval = 5f; // Time interval between spawns

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnEnemyLoop());
        StartCoroutine(SpawnPointsLoop());
    }

    private IEnumerator SpawnEnemyLoop()
    {
        while (true)
        {
            float spawnInterval = Random.Range(minEnemySpawnInterval, maxEnemySpawnInterval);
            yield return new WaitForSeconds(spawnInterval);

            // Instantiate the enemy at the spawner's position and rotation
            Instantiate(enemy, transform.position, transform.rotation);
        }
    }
    private IEnumerator SpawnPointsLoop()
    {
        while (true)
        {
            float spawnInterval = Random.Range(minPointsSpawnInterval, maxPointsSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);

            // Instantiate the points at the spawner's position and rotation
            Instantiate(points, transform.position, transform.rotation);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
