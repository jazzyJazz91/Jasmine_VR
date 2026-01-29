using UnityEngine;
using System.Collections.Generic;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Collectible Prefabs")]
    public List<GameObject> collectibles; // Lägg in coin, star, diamond här i Unity

    [Header("Spawn Settings")]
    public Vector3 spawnAreaMin = new Vector3(-5, 1, -5);
    public Vector3 spawnAreaMax = new Vector3(5, 3, 5);
    public float spawnInterval = 3f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCollectible), 2f, spawnInterval);
    }

    void SpawnCollectible()
    {
        if (collectibles.Count == 0) return;

        int randomIndex = Random.Range(0, collectibles.Count);
        GameObject prefab = collectibles[randomIndex];

        Vector3 randomPos = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );

        Instantiate(prefab, randomPos, prefab.transform.rotation);
    }
}
