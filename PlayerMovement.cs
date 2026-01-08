using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları - Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;

    [Header("Arayüz Ayarları - UI")]
    public GameObject gameOverText;   // Game Over yazısı
    public GameObject finishFlagImage; // FinishPoint içindeki bayrak resmi
    public Button restartButton;      // Restart butonu
    public TextMeshProUGUI scoreText; // Coin yazısı

    [Header("Ses Ayarları - Audio")]
    private AudioSource audioSource;
    public AudioClip jumpSound;     // Zıplama sesi
    public AudioClip coinSound;     // Altın sesi
    public AudioClip deathSound;    // Ölme sesi
    public AudioClip winSound;      // Kazanma sesi

    [Header("Oyun Durumu")]
    public float restartDelay = 1.5f; 
    private bool isDead = false;
    private bool isFinished = false;
    private int currentScore = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // AudioSource bileşenini al veya ekle
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        // Başlangıçta her şeyi gizle
        if (gameOverText) gameOverText.SetActive(false);
        if (finishFlagImage) finishFlagImage.SetActive(false); 
        if (restartButton) restartButton.gameObject.SetActive(false);
        
        if (restartButton) restartButton.onClick.AddListener(RestartGame);
        UpdateScoreUI();
    }

    void Update()
    {
        if (isDead || isFinished) return;

        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            PlaySound(jumpSound); // Zıplama sesini çal
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || isFinished) return;

        // 1. Bayrağa (FinishPoint) değdiğinde
        if (other.CompareTag("Finish"))
        {
            TriggerWin();
        }
        // 2. Ölüm bölgesine veya düşmana değdiğinde
        else if (other.CompareTag("DeathZone") || other.CompareTag("Trap") || other.CompareTag("Enemy"))
        {
            TriggerGameOver();
        }
        // 3. Altın topladığında
        else if (other.CompareTag("Coin"))
        {
            currentScore += 10;
            UpdateScoreUI();
            PlaySound(coinSound); // Altın sesini çal
            Destroy(other.gameObject);
        }
    }

    void TriggerWin()
    {
        if (isFinished) return;
        isFinished = true;
        FreezePlayer();

        // Bayrak resmini göster
        if (finishFlagImage != null) finishFlagImage.SetActive(true);

        PlaySound(winSound); // Kazanma sesini çal
        Debug.Log("Kazandın!");
        Invoke("ShowRestartButton", restartDelay);
    }

    void TriggerGameOver()
    {
        if (isDead) return;
        isDead = true;
        FreezePlayer();

        PlaySound(deathSound); // Ölme sesini çal
        if (gameOverText) gameOverText.SetActive(true);
        Invoke("ShowRestartButton", restartDelay);
    }

    void FreezePlayer()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static; 
        rb.simulated = false; 
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Coin: " + currentScore.ToString();
    }

    void ShowRestartButton()
    {
        if (restartButton) restartButton.gameObject.SetActive(true);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null) 
            audioSource.PlayOneShot(clip);
    }
}