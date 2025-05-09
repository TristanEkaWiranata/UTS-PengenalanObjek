using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DragonBallGameManager : MonoBehaviour
{
    public static DragonBallGameManager instance; // Singleton
    public static string lastGameOverMessage;     // Digunakan oleh scene GameOver

    public enum GameMode { PVP, PVE }
    public GameMode currentMode;

    [Header("Game Settings")]
    public float gameTime = 120f;

    [Header("UI References")]
    public Text timerText;

    [Header("Audio")]
    public AudioSource bgmAudioSource;

    private bool gameEnded = false;
    public int player1Score = 0;
    public int player2Score = 0;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Ambil mode dari PlayerPrefs
        string mode = PlayerPrefs.GetString("GameMode", "PVP"); // Default to PVP
        currentMode = (mode == "PVE") ? GameMode.PVE : GameMode.PVP;

        // Setup BGM
        if (bgmAudioSource != null)
        {
            DontDestroyOnLoad(bgmAudioSource.gameObject);
            if (!bgmAudioSource.isPlaying)
                bgmAudioSource.Play();
        }
    }

    void Update()
    {
        if (gameEnded) return;

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

    public void SetGameMode(GameMode mode)
    {
        if (currentMode != mode)  // Pastikan mode tidak berubah terus-menerus
        {
            currentMode = mode;
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
        lastGameOverMessage = reason;

        if (bgmAudioSource != null)
            bgmAudioSource.Stop();

        Time.timeScale = 1f;
        SceneManager.LoadScene("GameOverDragonBall");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameSceneDragonBall");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameSelection");
    }
}
