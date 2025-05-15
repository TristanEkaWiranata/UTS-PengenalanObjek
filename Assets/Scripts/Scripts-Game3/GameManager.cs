using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Text levelText;
    public Text timerText;
    public Image[] hearts;
    public GameObject winPanel;
    public Text winText;
    public Button playAgainButton;
    public Button exitButton;
    public GameObject difficultyPanel;
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;
    public AudioClip levelUpSound;
    public AudioSource audioSource;

    private int currentLevel = 1;
    private int score = 0;
    private int lives = 3;
    private float timeRemaining;
    public bool isGameActive = false;
    private bool listenersAdded = false;
    private string selectedDifficulty = "Medium";

    private readonly int[] scoreThresholds = { 200, 400, 700, 1000 };
    private readonly float[] spawnIntervalsBase = { 1.0f, 0.8f, 0.67f, 0.6f, 0.5f };
    private readonly float[] objectSpeedsBase = { 4.0f, 5.0f, 6.0f, 6.2f, 6.5f };
    private readonly int[] objectsToSortBase = { 100, 200, 300, 400, 500 };
    private readonly float[] levelTimesBase = { 60f, 50f, 40f, 30f, 20f };

    private float[] spawnIntervals;
    private float[] objectSpeeds;
    private int[] objectsToSort;
    private float[] levelTimes;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Duplikat GameManager pada {gameObject.name}. Menghapus instance ini.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Inisialisasi array dengan nilai default
        spawnIntervals = spawnIntervalsBase;
        objectSpeeds = objectSpeedsBase;
        objectsToSort = objectsToSortBase;
        levelTimes = levelTimesBase;
        // DontDestroyOnLoad(gameObject);
        Debug.Log("GameManager Awake: Instance diinisialisasi.");
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
            Debug.Log("Scene Game3Scene dimuat, menginisialisasi UI.");
            InitializeUI();
            // Hanya tampilkan panel kesulitan, jangan reset spawner/detektor dulu
            ShowDifficultyPanel();
        }
        else
        {
            levelText = null;
            timerText = null;
            for (int i = 0; i < hearts.Length; i++)
            {
                hearts[i] = null;
            }
            winPanel = null;
            winText = null;
            difficultyPanel = null;
            easyButton = null;
            mediumButton = null;
            hardButton = null;

            if (playAgainButton != null)
                playAgainButton.onClick.RemoveAllListeners();
            playAgainButton = null;

            if (exitButton != null)
                exitButton.onClick.RemoveAllListeners();
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
        // ResetGame tidak dipanggil di Start, karena kita tunggu pemilihan kesulitan
        Debug.Log("GameManager Start: Menunggu pemilihan kesulitan.");
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
            isGameActive = false;

            PlayerPrefs.SetInt("LastScore", score);
            PlayerPrefs.SetInt("LastLevel", currentLevel);

            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            if (score > highScore)
            {
                PlayerPrefs.SetInt("HighScore", score);
            }

            PlayerPrefs.Save();
            SceneManager.LoadScene("GameOver");
        }
    }

    public void TriggerGameWin()
    {
        if (isGameActive)
        {
            timeRemaining = 0;
            isGameActive = false;
            SaveHighScore();

            MunculObjekAntariksa spawner = FindFirstObjectByType<MunculObjekAntariksa>();
            if (spawner != null)
            {
                spawner.enabled = false;
                Debug.Log("Spawner dinonaktifkan saat menang.");
            }

            GerakObjekAntariksa[] movingObjects = FindObjectsByType<GerakObjekAntariksa>(FindObjectsSortMode.None);
            foreach (var obj in movingObjects)
            {
                obj.StopMovement();
            }

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

                if (!listenersAdded)
                {
                    if (playAgainButton != null)
                    {
                        playAgainButton.onClick.RemoveAllListeners();
                        playAgainButton.onClick.AddListener(PlayAgain);
                    }

                    if (exitButton != null)
                    {
                        exitButton.onClick.RemoveAllListeners();
                        exitButton.onClick.AddListener(ExitGame);
                    }

                    listenersAdded = true;
                }
            }
            else
            {
                Debug.LogWarning("WinPanel tidak ditemukan.");
            }
        }
    }

    void PlayAgain()
    {
        Debug.Log("Button Play Again ditekan.");
        ResetGame();
        SceneManager.LoadScene("Game3Scene");
    }

    void ExitGame()
    {
        Debug.Log("Quit ditekan. Kembali ke GameSelection.");
        ResetGame();
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
        isGameActive = false;
        listenersAdded = false;

        if (SceneManager.GetActiveScene().name == "Game3Scene")
        {
            InitializeUI();
            UpdateLivesUI();
            ShowDifficultyPanel(); // Tampilkan panel kesulitan lagi
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
            Debug.LogWarning("LevelText tidak ditemukan.");
        }

        GameObject timerTextObj = GameObject.Find("TimerText");
        if (timerTextObj != null)
        {
            timerText = timerTextObj.GetComponent<Text>();
        }
        else
        {
            Debug.LogWarning("TimerText tidak ditemukan.");
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
                Debug.LogWarning($"Heart{i + 1} tidak ditemukan.");
            }
        }

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
            Debug.LogWarning("WinPanel tidak ditemukan.");
        }

        GameObject difficultyPanelObj = GameObject.Find("DifficultyPanel");
        if (difficultyPanelObj != null)
        {
            difficultyPanel = difficultyPanelObj;
            difficultyPanel.SetActive(false);

            Transform easyButtonTransform = difficultyPanelObj.transform.Find("EasyButton");
            if (easyButtonTransform != null)
            {
                easyButton = easyButtonTransform.GetComponent<Button>();
                easyButton.onClick.RemoveAllListeners();
                easyButton.onClick.AddListener(() => SelectDifficulty("Easy"));
            }
            else
            {
                Debug.LogWarning("EasyButton tidak ditemukan di DifficultyPanel.");
            }

            Transform mediumButtonTransform = difficultyPanelObj.transform.Find("MediumButton");
            if (mediumButtonTransform != null)
            {
                mediumButton = mediumButtonTransform.GetComponent<Button>();
                mediumButton.onClick.RemoveAllListeners();
                mediumButton.onClick.AddListener(() => SelectDifficulty("Medium"));
            }
            else
            {
                Debug.LogWarning("MediumButton tidak ditemukan di DifficultyPanel.");
            }

            Transform hardButtonTransform = difficultyPanelObj.transform.Find("HardButton");
            if (hardButtonTransform != null)
            {
                hardButton = hardButtonTransform.GetComponent<Button>();
                hardButton.onClick.RemoveAllListeners();
                hardButton.onClick.AddListener(() => SelectDifficulty("Hard"));
            }
            else
            {
                Debug.LogWarning("HardButton tidak ditemukan di DifficultyPanel.");
            }
        }
        else
        {
            Debug.LogWarning("DifficultyPanel tidak ditemukan. Memulai dengan kesulitan default (Medium).");
            SelectDifficulty("Medium");
        }

        UpdateLevelUI();
        UpdateTimerUI();
        UpdateLivesUI();
    }

    private void ShowDifficultyPanel()
    {
        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(true);
            isGameActive = false;
            Debug.Log("Menampilkan panel pemilihan kesulitan.");
        }
        else
        {
            Debug.LogWarning("DifficultyPanel tidak ditemukan, memulai dengan kesulitan default (Medium).");
            SelectDifficulty("Medium");
        }
    }

    private void SelectDifficulty(string difficulty)
    {
        selectedDifficulty = difficulty;
        Debug.Log($"Kesulitan dipilih: {difficulty}");

        switch (difficulty)
        {
            case "Easy":
                spawnIntervals = new float[spawnIntervalsBase.Length];
                objectSpeeds = new float[objectSpeedsBase.Length];
                objectsToSort = new int[objectsToSortBase.Length];
                levelTimes = new float[levelTimesBase.Length];
                for (int i = 0; i < spawnIntervalsBase.Length; i++)
                {
                    spawnIntervals[i] = spawnIntervalsBase[i] * 1.5f;
                    objectSpeeds[i] = objectSpeedsBase[i] * 0.8f;
                    objectsToSort[i] = Mathf.FloorToInt(objectsToSortBase[i] * 0.7f);
                    levelTimes[i] = levelTimesBase[i] * 1.2f;
                }
                lives = 3;
                break;
            case "Medium":
                spawnIntervals = spawnIntervalsBase;
                objectSpeeds = objectSpeedsBase;
                objectsToSort = objectsToSortBase;
                levelTimes = levelTimesBase;
                lives = 3;
                break;
            case "Hard":
                spawnIntervals = new float[spawnIntervalsBase.Length];
                objectSpeeds = new float[objectSpeedsBase.Length];
                objectsToSort = new int[objectsToSortBase.Length];
                levelTimes = new float[levelTimesBase.Length];
                for (int i = 0; i < spawnIntervalsBase.Length; i++)
                {
                    spawnIntervals[i] = spawnIntervalsBase[i] * 0.7f;
                    objectSpeeds[i] = objectSpeedsBase[i] * 1.2f;
                    objectsToSort[i] = Mathf.FloorToInt(objectsToSortBase[i] * 1.3f);
                    levelTimes[i] = levelTimesBase[i] * 0.8f;
                }
                lives = 3;
                break;
        }

        timeRemaining = levelTimes[currentLevel - 1];
        UpdateLivesUI();
        UpdateTimerUI();
        UpdateGameDifficulty();

        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(false);
        }

        // Inisialisasi game setelah kesulitan dipilih
        InitializeGame();
        isGameActive = true;
        Debug.Log($"Permainan dimulai dengan kesulitan {difficulty}.");
    }

    private void InitializeGame()
    {
        // Inisialisasi spawner dan detektor hanya setelah kesulitan dipilih
        ResetSpawner();
        ResetDetectors();
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
            levelText.text = "Level " + currentLevel;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
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