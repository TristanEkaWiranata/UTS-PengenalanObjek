using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DragonBallGameManager : MonoBehaviour
{
    public static DragonBallGameManager instance; // Singleton

    public float gameTime = 120f;
    public Text timerText;
    public GameObject gameOverUI;  // UI untuk Game Over
    public Text gameOverText;  // Teks untuk menampilkan pesan Game Over (Pemenang atau Waktu Habis)
    private bool gameEnded = false;

    public int player1Score = 0;
    public int player2Score = 0;

    // Tambahkan referensi untuk AudioSource BGM
    public AudioSource bgmAudioSource;

    // Referensi untuk tombol Restart dan Quit
    public Button restartButton;
    public Button quitButton;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
        {
            if (bgmAudioSource != null)
        {
            DontDestroyOnLoad(bgmAudioSource.gameObject);
        }

        // Pastikan Game Over UI dinonaktifkan pada awal permainan
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false); // Menyembunyikan UI Game Over pada saat game mulai
        }

        // Menambahkan listener untuk tombol restart dan quit
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame); // Ketika tombol restart diklik, panggil RestartGame
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame); // Ketika tombol quit diklik, panggil QuitGame
        }if (bgmAudioSource != null)
        {
            DontDestroyOnLoad(bgmAudioSource.gameObject);
        }

        // Pastikan Game Over UI dinonaktifkan pada awal permainan
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false); // Menyembunyikan UI Game Over pada saat game mulai
        }

        // Menambahkan listener untuk tombol restart dan quit
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame); // Ketika tombol restart diklik, panggil RestartGame
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame); // Ketika tombol quit diklik, panggil QuitGame
        }
    }

    void Update()
    {
        if (gameEnded) return;  // Jangan update jika game sudah berakhir

        gameTime -= Time.deltaTime;

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        if (gameTime <= 0f)
        {
            GameOver("Waktu habis!");
        }

        if (player1Score >= 7)
        {
            GameOver("Player 1 menang!");
        }
        else if (player2Score >= 7)
        {
            GameOver("Player 2 menang!");
        }
    }

    public void AddScore(int playerNumber)
    {
        if (playerNumber == 1)
            player1Score++;
        else if (playerNumber == 2)
            player2Score++;
    }

    void GameOver(string reason)
    {
        gameEnded = true;
        Debug.Log("Game Over: " + reason);

        // Hentikan BGM saat game berakhir
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop(); // Stop the BGM
        }

        // Menampilkan Game Over UI dan menampilkan pesan pemenang
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);  // Mengaktifkan UI Game Over
            if (gameOverText != null)
            {
                gameOverText.text = reason;  // Menampilkan pesan Game Over (pemenang atau waktu habis)
            }
        }

        Time.timeScale = 0f; // Freeze game
    }

    // Fungsi untuk Restart game
    public void RestartGame()
    {
        Time.timeScale = 1f;  // Restart permainan
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGame();

        SceneManager.LoadScene("GameSceneDragonBall");
    }

    // Fungsi untuk Quit game
    public void QuitGame()
    {
        Debug.Log("Exiting Game...");
        SceneManager.LoadScene("GameSelection");
    }
}
