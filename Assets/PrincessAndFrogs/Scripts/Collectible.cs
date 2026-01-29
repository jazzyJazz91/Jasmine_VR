using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType { Coin, Star, Diamond }
    public CollectibleType type;

    private int points;

    [Header("Effects")]
    public AudioClip collectSound;
    public ParticleSystem collectEffect;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Sätt poäng beroende på typ
        switch (type)
        {
            case CollectibleType.Coin:
                points = 2;
                break;
            case CollectibleType.Star:
                points = 5;
                break;
            case CollectibleType.Diamond:
                points = 10;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Lägg till poäng
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(points);

            // Spela upp ljud
            if (collectSound != null && audioSource != null)
                audioSource.PlayOneShot(collectSound);

            // Skapa partiklar
            if (collectEffect != null)
                Instantiate(collectEffect, transform.position, Quaternion.identity);

            // Förstör objektet efter ljudet spelats (0.1s delay)
            Destroy(gameObject, 0.1f);
        }
    }
}
