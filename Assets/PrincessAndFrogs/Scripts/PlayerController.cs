using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    private Rigidbody playerRb;
    private Animator playerAnim;
    private AudioSource playerAudio;

    [Header("Movement & Jumping")]
    public float jumpForce = 10f;
    public float gravityModifier = 1f;
    public bool isOnGround = true;
    public bool gameOver = false;   // används även vid win
    public int maxJumps = 2;

    [Header("Particles & Sounds")]
    public ParticleSystem dirtParticle;
    public ParticleSystem explosionParticle;
    public AudioClip jumpSound;
    public AudioClip crashSound;

    [Header("Health System")]
    public int maxHealth = 3;
    private int currentHealth;
    public Slider healthSlider;
    public Gradient healthGradient;
    public Image fillImage;

    [Header("Stomp")]
    public ParticleSystem stompExplosionPrefab;
    public float stompBounceForce = 8f;

    [Header("UI")]
    public TextMeshProUGUI gameOverText;
    public Button restartButton;

    [Header("Score / Win")]
    public int targetScore = 100;     // 🏆 vinstgräns
    public int currentScore = 0;
    public TextMeshProUGUI scoreText; // visa aktuell poäng
    public TextMeshProUGUI winText;   // stor “YOU WIN!”

    [Header("Coin Values")]
    public int coinValue = 2;
    public int starValue = 5;
    public int diamondValue = 10;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();

        Physics.gravity = new Vector3(0, -9.81f * gravityModifier, 0);

        currentHealth = maxHealth;
        UpdateHealthBar();

        // Dölj Game Over och knappen
        if (gameOverText != null) gameOverText.gameObject.SetActive(false);
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
            restartButton.onClick.AddListener(RestartGame);
        }

        // Initiera score UI & dölj vinsttext
        UpdateScoreUI();
        if (winText != null) winText.gameObject.SetActive(false);

        StartCoroutine(RegenerateHealth());
    }

    void Update()
    {
        if (gameOver) return;

        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            if (jumpSound && playerAudio) playerAudio.PlayOneShot(jumpSound, 1.0f);
            if (dirtParticle) dirtParticle.Stop();

            playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (playerAnim) playerAnim.SetTrigger("Jump_trig");
            isOnGround = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (dirtParticle) dirtParticle.Play();
            isOnGround = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            if (crashSound && playerAudio) playerAudio.PlayOneShot(crashSound, 1.0f);
            if (dirtParticle) dirtParticle.Stop();
            TakeDamage(1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // --- COINS / STARS / DIAMONDS ---
        int add = 0;
        if (other.CompareTag("Coin")) add = coinValue;
        else if (other.CompareTag("Star")) add = starValue;
        else if (other.CompareTag("Diamond")) add = diamondValue;

        if (add > 0)
        {
            AddScore(add);
            Debug.Log($"[PICKUP] {other.tag} +{add} → score={currentScore}/{targetScore}");
            Destroy(other.gameObject);
            return;
        }

        // --- STOMP (ingen poäng här) ---
        if (other.CompareTag("StompZone"))
        {
            Transform enemyRoot = other.transform.root;

            if (stompExplosionPrefab != null)
                Instantiate(stompExplosionPrefab, enemyRoot.position, Quaternion.identity);

            playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
            playerRb.AddForce(Vector3.up * stompBounceForce, ForceMode.Impulse);

            Destroy(enemyRoot.gameObject);
            return;
        }

        // Annan trigger som skadar spelaren (t.ex. FrogBody)
        var frog = other.GetComponent<FrogBody>();
        if (frog != null)
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }

    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            TriggerGameOver();
        }
        else
        {
            if (playerAnim) playerAnim.SetTrigger("Hit");
        }
    }

    // ---------- SCORE / WIN ----------
    private void AddScore(int amount)
    {
        if (gameOver) return;

        currentScore += amount;
        UpdateScoreUI();

        if (currentScore >= targetScore)
            TriggerWin();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";
    }

    private void TriggerWin()
    {
        if (gameOver) return;

        gameOver = true;
        Time.timeScale = 0f;

        if (winText != null)
        {
            winText.text = "YOU WIN!";
            winText.gameObject.SetActive(true);
        }
        else if (gameOverText != null)
        {
            // fallback om winText inte finns kopplad
            gameOverText.text = "YOU WIN!";
            gameOverText.gameObject.SetActive(true);
        }

        if (restartButton != null) restartButton.gameObject.SetActive(true);
        Debug.Log("[WIN] TriggerWin() kördes och vinst-UI aktiverades.");
    }
    // ----------------------------------

    private void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;

            if (fillImage != null && healthGradient != null)
                fillImage.color = healthGradient.Evaluate((float)currentHealth / maxHealth);
        }
    }

    private void TriggerGameOver()
    {
        if (gameOver) return;

        gameOver = true;
        Debug.Log("GAME OVER – alla liv slut!");

        if (playerAnim)
        {
            playerAnim.SetBool("Death_b", true);
            playerAnim.SetInteger("DeathType_int", 1);
        }

        if (gameOverText != null) gameOverText.gameObject.SetActive(true);
        if (restartButton != null) restartButton.gameObject.SetActive(true);

        Time.timeScale = 0f;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator RegenerateHealth()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(20f);

            if (currentHealth < maxHealth)
            {
                currentHealth++;
                UpdateHealthBar();
                Debug.Log("Player regained 1 health. Current health: " + currentHealth);
            }
        }
    }
}
