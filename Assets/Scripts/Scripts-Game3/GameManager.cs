using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Text levelText; // UI untuk level
    public Text timerText; // UI untuk timer
    public AudioClip levelUpSound;
    private AudioSource audioSource;

    private int currentLevel = 1;
    private int score = 0;
    private float timeRemaining;
    private bool isGameActive = false; // Mulai dengan false
    private readonly int[] scoreThresholds = { 200, 400 };
    private readonly float[] spawnIntervals = { 1.0f, 0.8f, 0.67f };
    private readonly float[] objectSpeeds = { 4.0f, 5.0f, 6.0f };
    private readonly int[] objectsToSort = { 20, 25, 30 };
    private readonly float[] levelTimes = { 60f, 50f, 40f };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Duplikat GameManager ditemukan pada {gameObject.name}. Menghapus instance ini.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameManager diinisialisasi sebagai singleton.");
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = levelUpSound;
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

    public void AddScore(int points)
    {
        score += points;
        if (score < 0) score = 0;
        CheckLevelUp(score);
        Debug.Log($"Skor diperbarui: {score} (+{points})");
    }

    public void ResetScore()
    {
        score = 0;
        Debug.Log("Skor direset ke 0.");
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetGame()
    {
        Debug.Log("ResetGame: Memulai...");
        score = 0;
        currentLevel = 1;
        timeRemaining = levelTimes[currentLevel - 1];
        isGameActive = true;
        InitializeUI();
        ResetSpawner();
        ResetDetectors();
        Debug.Log($"ResetGame selesai: Skor={score}, Level={currentLevel}, Timer={timeRemaining}, Active={isGameActive}");
    }

    private void InitializeUI()
    {
        Debug.Log("InitializeUI: Mencari UI di Game3Scene...");
        // Cari LevelText
        GameObject levelTextObj = GameObject.Find("LevelText") ??
                                 GameObject.FindGameObjectWithTag("LevelText") ??
                                 GameObject.Find("Level");
        if (levelTextObj == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Text[] texts = canvas.GetComponentsInChildren<Text>(true);
                foreach (Text text in texts)
                {
                    if (text.name.ToLower().Contains("level"))
                    {
                        levelTextObj = text.gameObject;
                        Debug.Log($"LevelText ditemukan: {levelTextObj.name}");
                        break;
                    }
                }
            }
        }
        if (levelTextObj != null)
        {
            levelText = levelTextObj.GetComponent<Text>();
            Debug.Log($"LevelText diatur: {levelTextObj.name}");
        }
        else
        {
            Debug.LogWarning("LevelText tidak ditemukan. UI Level mungkin tidak diperbarui.");
        }

        // Cari TimerText
        GameObject timerTextObj = GameObject.Find("TimerText") ??
                                 GameObject.FindGameObjectWithTag("TimerText") ??
                                 GameObject.Find("Timer");
        if (timerTextObj == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Text[] texts = canvas.GetComponentsInChildren<Text>(true);
                foreach (Text text in texts)
                {
                    if (text.name.ToLower().Contains("timer") || text.name.ToLower().Contains("time"))
                    {
                        timerTextObj = text.gameObject;
                        Debug.Log($"TimerText ditemukan: {timerTextObj.name}");
                        break;
                    }
                }
            }
        }
        if (timerTextObj != null)
        {
            timerText = timerTextObj.GetComponent<Text>();
            Debug.Log($"TimerText diatur: {timerTextObj.name}");
        }
        else
        {
            Debug.LogWarning("TimerText tidak ditemukan. UI Timer mungkin tidak diperbarui.");
        }

        UpdateLevelUI();
        UpdateTimerUI();
    }

    private void ResetSpawner()
    {
        MunculObjekAntariksa spawner = FindFirstObjectByType<MunculObjekAntariksa>();
        if (spawner != null)
        {
            spawner.ResetSpawner();
            Debug.Log("Spawner direset.");
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
            Debug.Log($"Detektor {detector.gameObject.name} direset.");
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
                audioSource.Play();
                UpdateLevelUI();
                UpdateGameDifficulty();
                Debug.Log($"Level naik ke {currentLevel}, Timer={timeRemaining}");
                break;
            }
        }
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
            Debug.Log($"Level UI diperbarui: Level={currentLevel}");
        }
        else
        {
            Debug.LogWarning("LevelText null, tidak bisa memperbarui Level UI.");
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();
            timerText.color = timeRemaining < 10f ? Color.red : Color.white;
        }
        else
        {
            Debug.LogWarning("TimerText null, tidak bisa memperbarui Timer UI.");
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
}