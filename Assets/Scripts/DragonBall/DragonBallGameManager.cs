using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DragonBallGameManager : MonoBehaviour
{
    public static DragonBallGameManager instance;
    public static string lastGameOverMessage;

    public enum GameMode { PVP, PVE }
    public GameMode currentMode;

    [Header("Game Settings")]
    public float maxGameTime = 60f;
    private float currentGameTime;

    [Header("UI References")]
    public Text timerText;

    [Header("Audio")]
    public AudioSource bgmAudioSource;

    private bool gameEnded;
    private int player1Score;
    private int player2Score;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); // hanya hancurkan jika bukan instance yang aktif
        }
    }


    void Start()
    {
        string gameMode = PlayerPrefs.GetString("GameMode", "PVP"); // Default ke PVP jika tidak ada nilai
        if (gameMode == "PVE")
        {
            currentMode = GameMode.PVE;
        }
        else
        {
            currentMode = GameMode.PVP;
        }
        
        InitializeGame();
    }

    void InitializeGame()
    {
        Debug.Log("InitializeGame dipanggil");

        gameEnded = false;
        player1Score = 0;
        player2Score = 0;
        currentGameTime = maxGameTime;
        Time.timeScale = 1f;

        // Pastikan referensi UI dan Audio valid
        if (timerText == null)
            timerText = GameObject.FindWithTag("TimerText")?.GetComponent<Text>();

        if (bgmAudioSource == null)
            bgmAudioSource = GameObject.FindWithTag("BGM")?.GetComponent<AudioSource>();

        if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
            bgmAudioSource.Play();
    }

    void Update()
    {
        if (gameEnded) return;

        currentGameTime -= Time.deltaTime;
        UpdateTimerDisplay();

        if (currentGameTime <= 0f)
        {
            if (player1Score == player2Score)
                GameOver("Seri!");
            else if (player1Score > player2Score)
                GameOver("Player 1 menang!");
            else
                GameOver("Player 2 menang!");
        }
        else if (player1Score >= 7)
        {
            GameOver("Player 1 menang!");
        }
        else if (player2Score >= 7)
        {
            GameOver("Player 2 menang!");
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentGameTime / 60);
            int seconds = Mathf.FloorToInt(currentGameTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void RestartGame()
    {
        StartCoroutine(RestartAndReassignReferences());
    }

    private IEnumerator RestartAndReassignReferences()
    {
        SceneManager.LoadScene("GameSceneDragonBall");
        yield return new WaitForSeconds(0.1f); // Tunggu scene selesai load

        // Ambil ulang referensi UI & audio dari scene baru
        timerText = GameObject.FindWithTag("TimerText")?.GetComponent<Text>();
        bgmAudioSource = GameObject.FindWithTag("BGM")?.GetComponent<AudioSource>();

        InitializeGame();
    }

    public void SetGameMode(GameMode mode)
    {
        if (currentMode != mode)
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

   public void ExitToMenu()
    {
        Destroy(gameObject);
        StartCoroutine(QuitAndResetState());
    }

    private IEnumerator QuitAndResetState()
    {
        SceneManager.LoadScene("VersusSelection");
        yield return new WaitForSeconds(0.1f);

        // Reset semua nilai agar seperti baru
        timerText = null;
        bgmAudioSource = null;
        player1Score = 0;
        player2Score = 0;
        currentGameTime = maxGameTime;
        gameEnded = false;

        // Stop musik jika ada
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
            bgmAudioSource.Stop();
    }

}
