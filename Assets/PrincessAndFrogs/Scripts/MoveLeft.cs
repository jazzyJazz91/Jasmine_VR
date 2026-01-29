using UnityEngine;
public class MoveLeft : MonoBehaviour
{
    public static float globalSpeedMultiplier = 1f; // reset i SpawnManager.Start()
    [Min(0f)] public float baseSpeed = 30f;

    private PlayerController playerControllerScript;
    private float leftBound = -15f;

    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!playerControllerScript.gameOver)
        {
            float currentSpeed = baseSpeed * Mathf.Max(0.01f, globalSpeedMultiplier);
            transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);
        }

        if (transform.position.x < leftBound)
            Destroy(gameObject);
    }
}
