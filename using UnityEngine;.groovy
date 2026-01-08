using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("إعدادات الحركة")]
    public float speed = 8f;
    public float jumpForce = 18f;

    [Header("إعدادات الواجهة")]
    public TextMeshProUGUI scoreText;
    private int coinsCount = 0;

    [Header("الأصوات (SFX)")]
    public AudioSource sfxPlayer; 
    public AudioClip jumpSound;
    public AudioClip coinSound;
    public AudioClip deathSound;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDead = false; // لمنع الحركة بعد الموت

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 4f;

        // تأكد برمجياً أن الصوت لا يعمل تلقائياً عند البداية
        if (sfxPlayer != null)
        {
            sfxPlayer.playOnAwake = false;
            sfxPlayer.stop(); 
        }

        UpdateScoreUI();
    }

    void Update()
    {
        if (isDead) return; // إذا مات اللاعب يتوقف التحكم

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if(jumpSound && sfxPlayer) sfxPlayer.PlayOneShot(jumpSound);
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // جمع العملات
        if (other.gameObject.CompareTag("Coin"))
        {
            coinsCount++;
            UpdateScoreUI();
            if(coinSound && sfxPlayer) sfxPlayer.PlayOneShot(coinSound);
            Destroy(other.gameObject);
        }

        // الموت عند لمس الأشواك
        if (other.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero; // إيقاف حركة اللاعب فوراً
        
        if(deathSound && sfxPlayer) sfxPlayer.PlayOneShot(deathSound);
        
        // إعادة تحميل المرحلة بعد ثانية واحدة (لإعطاء وقت لسماع الصوت)
        Invoke("RestartLevel", 1f);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Coins: " + coinsCount;
        }
    }
}