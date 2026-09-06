using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    [Header("Bomb Settings")]
    public GameObject bombPrefab;
    public float spawnInterval = 2f;

    private static List<BombSpawner> spawners = new List<BombSpawner>();
    private static bool loopRunning = false;

    private void Awake()
    {
        if (!spawners.Contains(this))
            spawners.Add(this);

        if (!loopRunning)
        {
            loopRunning = true;
            StartCoroutine(SpawnLoop());
        }
    }

    private void OnDestroy()
    {
        spawners.Remove(this);

        if (spawners.Count == 0)
            loopRunning = false;
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (spawners.Count == 0)
                continue;

            BombSpawner randomSpawner = spawners[Random.Range(0, spawners.Count)];

            if (randomSpawner.bombPrefab != null)
            {
                Instantiate(
                    randomSpawner.bombPrefab,
                    randomSpawner.transform.position,
                    Quaternion.identity
                );
            }
        }
    }
}