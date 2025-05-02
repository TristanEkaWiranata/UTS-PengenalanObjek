using UnityEngine;
using UnityEngine.UI;

public class BallGoalDetector : MonoBehaviour
{
    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;
    public Text Player1Score;
    public Text Player2Score;
    public GameObject ball;  // Referensi ke objek bola
    public Vector2 resetPosition = new Vector2(0, 0);  // Posisi tengah
    private Rigidbody2D ballRigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ball == null)
        {
            Debug.LogError("Ball object is not assigned.");
        }
        
        // Ambil referensi ke Rigidbody2D bola
        ballRigidbody = ball.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Player1Score == null || Player2Score == null)
        {
            Debug.LogError("Player score Text components are not assigned.");
            return;
        }

        if (other.CompareTag("GoalPlayer1"))
        {
            scorePlayer2++;
            Player2Score.text = scorePlayer2.ToString();
            ResetBall();
        }
        else if (other.CompareTag("GoalPlayer2"))
        {
            scorePlayer1++;
            Player1Score.text = scorePlayer1.ToString();
            ResetBall();
        }
    }

    // Fungsi untuk mengatur bola kembali ke tengah
    void ResetBall()
    {
        if (ball != null && ballRigidbody != null)
        {
            // Matikan sementara gerakan bola dengan set kecepatan menjadi nol
            ballRigidbody.linearVelocity = Vector2.zero;
            ballRigidbody.angularVelocity = 0;

            // Atur posisi bola ke tengah
            ball.transform.position = resetPosition;

            // Jika ingin bola bergerak setelah beberapa detik, Anda bisa menggunakan ini:
            // Setelah beberapa detik, aktifkan kecepatan bola lagi atau gerakkan bola secara manual
            // Misalnya, Anda bisa memberikan sedikit gaya agar bola melanjutkan permainan.
            // ballRigidbody.velocity = new Vector2(1f, 0); // Gaya ke kanan untuk memulai kembali
        }
    }
}
