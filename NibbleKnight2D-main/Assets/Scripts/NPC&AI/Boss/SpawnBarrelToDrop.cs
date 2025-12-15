using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBarrelToDrop : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefabToSpawn;
    public Transform spawnPoint;

    private GameObject currentInstance;

    public float respawnDelay = 2f;
    private bool isRespawning = false;

    void Start()
    {
        SpawnPrefab();
    }

    void Update()
    {
        // If the object was destroyed, spawn a new one
        if (currentInstance == null && !isRespawning)
        {
            StartCoroutine(RespawnAfterDelay());
        }
    }

    void SpawnPrefab()
    {
        currentInstance = Instantiate(
            prefabToSpawn,
            spawnPoint.position,
            spawnPoint.rotation
        );
    }

    IEnumerator RespawnAfterDelay()
    {
        isRespawning = true;
        yield return new WaitForSeconds(respawnDelay);
        SpawnPrefab();
        isRespawning = false;
    }
}
