using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Text levelText;
    public Text timerText;
    public Image[] hearts;
    public GameObject winPanel; // Panel untuk UI kemenangan
    public Text winText; // Teks di dalam WinPanel
    public Button playAgainButton; // Tombol Play Again di WinPanel
    public Button exitButton; // Tombol Exit di WinPanel
    public AudioClip levelUpSound;
    public AudioSource audioSource;

    private int currentLevel = 1;
    private int score = 0;
    private int lives = 3;
    private float timeRemaining;
    private bool isGameActive = false;
    private readonly int[] scoreThresholds = { 200, 400, 700, 1000 };
    private readonly float[] spawnIntervals = { 1.0f, 0.8f, 0.67f, 0.6f, 0.5f };
    private readonly float[] objectSpeeds = { 4.0f, 5.0f, 6.0f, 6.2f, 6.5f };
    private readonly int[] objectsToSort = { 100, 200, 300, 400, 500 };
    private readonly float[] levelTimes = { 60f, 50f, 40f, 30f, 20f };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Duplikat GameManager pada {gameObject.name}. Menghapus instance ini.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game3Scene")
        {
            Debug.Log("Scene Game3Scene dimuat, menginisialisasi ulang UI.");
            InitializeUI();
            UpdateLevelUI();
            UpdateTimerUI();
            UpdateLivesUI();
            ResetSpawner();
            ResetDetectors();
        }
        else
        {
            levelText = null;
            timerText = null;
            hearts = new Image[3];
            winPanel = null;
            winText = null;
            playAgainButton = null;
            exitButton = null;
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (levelUpSound != null)
        {
            audioSource.clip = levelUpSound;
        }
        ResetGame();
        Debug.Log("GameManager Start: Game direset.");
    }

    void Update()
    {
        if (isGameActive && SceneManager.GetActiveScene().name == "Game3Scene")
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                TriggerGameOver();
            }
            UpdateTimerUI();
        }
    }

    public void LoseLife()
    {
        if (isGameActive)
        {
            lives--;
            UpdateLivesUI();
            if (lives <= 0)
            {
                TriggerGameOver();
            }
        }
    }

    public void TriggerGameOver()
    {
        if (isGameActive)
        {
            timeRemaining = 0;
            isGameActive = false;
            SaveHighScore();
            SceneManager.LoadScene("GameOver");
            Debug.Log("Game over, memuat scene GameOver.");
        }
    }

    public void TriggerGameWin()
    {
        if (isGameActive)
        {
            timeRemaining = 0;
            isGameActive = false;
            SaveHighScore();

            // Hentikan spawner
            MunculObjekAntariksa spawner = FindFirstObjectByType<MunculObjekAntariksa>();
            if (spawner != null)
            {
                spawner.enabled = false;
                Debug.Log("Spawner dinonaktifkan saat menang.");
            }

            // Hentikan semua objek yang sedang bergerak
            GerakObjekAntariksa[] movingObjects = FindObjectsByType<GerakObjekAntariksa>(FindObjectsSortMode.None);
            foreach (var obj in movingObjects)
            {
                obj.StopMovement(); // Pastikan kamu punya method ini
            }

            // Tampilkan UI kemenangan
            if (winPanel != null)
            {
                winPanel.SetActive(true);
                if (winText != null)
                {
                    winText.text = "Selamat, kamu telah menyelesaikan game!";
                }
                else
                {
                    Debug.LogWarning("winText tidak ditemukan di WinPanel.");
                }

                if (playAgainButton != null)
                {
                    playAgainButton.onClick.RemoveAllListeners();
                    playAgainButton.onClick.AddListener(PlayAgain);
                    Debug.Log("PlayAgainButton diatur.");
                }
                else
                {
                    Debug.LogWarning("PlayAgainButton tidak ditemukan di WinPanel.");
                }

                if (exitButton != null)
                {
                    exitButton.onClick.RemoveAllListeners();
                    exitButton.onClick.AddListener(ExitGame);
                    Debug.Log("ExitButton diatur.");
                }
                else
                {
                    Debug.LogWarning("ExitButton tidak ditemukan di WinPanel.");
                }
            }
            else
            {
                Debug.LogWarning("WinPanel tidak ditemukan, tidak bisa menampilkan UI kemenangan.");
            }
        }
    }


    void PlayAgain()
    {
        Debug.Log("Button Play Again ditekan.");
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGame();

        SceneManager.LoadScene("Game3Scene");
    }

    void ExitGame()
    {
        Debug.Log("Quit ditekan. Kembali ke GameSelection.");
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGame();
        SceneManager.LoadScene("GameSelection");
    }

    public void AddScore(int points)
    {
        score += points;
        if (score < 0) score = 0;
        CheckLevelUp(score);
    }

    public void ResetScore()
    {
        score = 0;
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetGame()
    {
        score = 0;
        currentLevel = 1;
        lives = 3;
        timeRemaining = levelTimes[currentLevel - 1];
        isGameActive = true;

        if (SceneManager.GetActiveScene().name == "Game3Scene")
        {
            InitializeUI();
            UpdateLivesUI();
            ResetSpawner();
            ResetDetectors();
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        Debug.Log($"ResetGame selesai: Skor={score}, Level={currentLevel}, Lives={lives}, Timer={timeRemaining}, Active={isGameActive}");
    }

    private void InitializeUI()
    {
        GameObject levelTextObj = GameObject.Find("LevelText");
        if (levelTextObj != null)
        {
            levelText = levelTextObj.GetComponent<Text>();
        }
        else
        {
            Debug.LogWarning("LevelText tidak ditemukan. Pastikan ada GameObject bernama 'LevelText' di Game3Scene.");
        }

        GameObject timerTextObj = GameObject.Find("TimerText");
        if (timerTextObj != null)
        {
            timerText = timerTextObj.GetComponent<Text>();
        }
        else
        {
            Debug.LogWarning("TimerText tidak ditemukan. Pastikan ada GameObject bernama 'TimerText' di Game3Scene.");
        }

        hearts = new Image[3];
        for (int i = 0; i < hearts.Length; i++)
        {
            GameObject heartObj = GameObject.Find($"Heart{i + 1}");
            if (heartObj != null)
            {
                hearts[i] = heartObj.GetComponent<Image>();
            }
            else
            {
                Debug.LogWarning($"Heart{i + 1} tidak ditemukan. Pastikan ada GameObject bernama 'Heart{i + 1}' di Game3Scene.");
            }
        }

        // Inisialisasi WinPanel dan komponennya
        GameObject winPanelObj = GameObject.Find("WinPanel");
        if (winPanelObj != null)
        {
            winPanel = winPanelObj;
            winPanel.SetActive(false);

            GameObject winTextObj = GameObject.Find("WinText");
            if (winTextObj != null)
            {
                winText = winTextObj.GetComponent<Text>();
            }
            else
            {
                Debug.LogWarning("WinText tidak ditemukan di WinPanel.");
            }

            GameObject playAgainButtonObj = GameObject.Find("PlayAgainButton");
            if (playAgainButtonObj != null)
            {
                playAgainButton = playAgainButtonObj.GetComponent<Button>();
            }
            else
            {
                Debug.LogWarning("PlayAgainButton tidak ditemukan di WinPanel.");
            }

            GameObject exitButtonObj = GameObject.Find("ExitButton");
            if (exitButtonObj != null)
            {
                exitButton = exitButtonObj.GetComponent<Button>();
            }
            else
            {
                Debug.LogWarning("ExitButton tidak ditemukan di WinPanel.");
            }
        }
        else
        {
            Debug.LogWarning("WinPanel tidak ditemukan. Pastikan ada GameObject bernama 'WinPanel' di Game3Scene.");
        }

        UpdateLevelUI();
        UpdateTimerUI();
        UpdateLivesUI();
    }

    private void ResetSpawner()
    {
        MunculObjekAntariksa spawner = FindFirstObjectByType<MunculObjekAntariksa>();
        if (spawner != null)
        {
            spawner.ResetSpawner();
        }
        else
        {
            Debug.LogWarning("MunculObjekAntariksa tidak ditemukan.");
        }
    }

    private void ResetDetectors()
    {
        DeteksiObjekAntariksa[] detectors = FindObjectsByType<DeteksiObjekAntariksa>(FindObjectsSortMode.None);
        foreach (var detector in detectors)
        {
            detector.ResetDetector();
        }
    }

    public void CheckLevelUp(int score)
    {
        for (int i = 0; i < scoreThresholds.Length; i++)
        {
            if (score >= scoreThresholds[i] && currentLevel < i + 2)
            {
                currentLevel = i + 2;
                timeRemaining = levelTimes[currentLevel - 1];
                if (audioSource != null && levelUpSound != null)
                {
                    audioSource.Play();
                }
                UpdateLevelUI();
                UpdateGameDifficulty();

                if (currentLevel >= 5)
                {
                    TriggerGameWin();
                }
                break;
            }
        }
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();
            timerText.color = timeRemaining < 10f ? Color.red : Color.white;
        }
    }

    private void UpdateLivesUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                hearts[i].gameObject.SetActive(i < lives);
            }
        }
    }

    private void UpdateGameDifficulty()
    {
        MunculObjekAntariksa munculSpawner = FindFirstObjectByType<MunculObjekAntariksa>();
        if (munculSpawner != null)
        {
            munculSpawner.ResetForNewLevel(spawnIntervals[currentLevel - 1]);
        }

        GerakObjekAntariksa[] objects = FindObjectsByType<GerakObjekAntariksa>(FindObjectsSortMode.None);
        foreach (var obj in objects)
        {
            obj.SetSpeed(objectSpeeds[currentLevel - 1]);
        }
        Debug.Log($"Kesulitan diperbarui: SpawnInterval={spawnIntervals[currentLevel - 1]}, Speed={objectSpeeds[currentLevel - 1]}");
    }

    public void SaveHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
            Debug.Log($"New High Score: {score}");
        }
    }

    public float GetSpawnInterval()
    {
        return spawnIntervals[currentLevel - 1];
    }

    public float GetObjectSpeed()
    {
        return objectSpeeds[currentLevel - 1];
    }

    public int GetObjectsToSort()
    {
        return objectsToSort[currentLevel - 1];
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public float GetLevelTime()
    {
        return levelTimes[currentLevel - 1];
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int GetLives()
    {
        return lives;
    }
}