using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [Header("Obstacle settings")]
    public GameObject[] obstacles;
    public Vector3 spawnPos = new Vector3(10, 0, 0);

    [Header("Timing settings")]
    public float minSpawnDelay = 1.0f;
    public float maxSpawnDelay = 3.0f;
    public float startDelay = 2.0f;

    [Header("Difficulty settings")]
    public float firstIncreaseAfter = 20f;
    public float increaseInterval = 20f;
    public float delayMultiplier = 0.85f;
    public float speedMultiplierIncrease = 1.1f; // speds 10% every time 
    public float minCapMin = 0.3f;
    public float minCapMax = 0.7f;

    [Header("Particles & Sounds")]
    public ParticleSystem dirtParticle;
    public ParticleSystem explosionParticle;
    private PlayerController playerControllerScript;

    void Start()
    {
    // Viktigt: starta alltid på normal hastighet
    MoveLeft.globalSpeedMultiplier = 1f;

    playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    StartCoroutine(SpawnObstacles());
    StartCoroutine(IncreaseDifficultyOverTime());
}

    IEnumerator SpawnObstacles()
    {
        yield return new WaitForSeconds(startDelay);

        while (!playerControllerScript.gameOver)
        {
            int index = Random.Range(0, obstacles.Length);
            Instantiate(obstacles[index], spawnPos, obstacles[index].transform.rotation);

            float randomDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    IEnumerator IncreaseDifficultyOverTime()
    {
        yield return new WaitForSeconds(firstIncreaseAfter);

        while (!playerControllerScript.gameOver)
        {
            // faster spawns
            minSpawnDelay = Mathf.Max(minCapMin, minSpawnDelay * delayMultiplier);
            maxSpawnDelay = Mathf.Max(minCapMax, maxSpawnDelay * delayMultiplier);

            // faster enemies
            MoveLeft.globalSpeedMultiplier *= speedMultiplierIncrease;

            Debug.Log($"[DIFFICULTY] Spawn: {minSpawnDelay:0.00}-{maxSpawnDelay:0.00}s | " +
                      $"Enemy speed x{MoveLeft.globalSpeedMultiplier:0.00}");

            yield return new WaitForSeconds(increaseInterval);
        }
    }
}
