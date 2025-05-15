using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

    [Header("Game Settings")]
    private int currentLevel = 1; // Level saat ini
    private int score = 0; // Skor pemain
    private int lives = 3; // Jumlah nyawa
    private float timeRemaining; // Waktu tersisa
    public bool isGameActive = false; // Status permainan
    private bool listenersAdded = false; // Status listener tombol
    private string selectedDifficulty = "Medium"; // Kesulitan default

    // Data level dan kesulitan
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
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Inisialisasi array
        spawnIntervals = spawnIntervalsBase;
        objectSpeeds = objectSpeedsBase;
        objectsToSort = objectsToSortBase;
        levelTimes = levelTimesBase;
    }
    #endregion

    #region Scene Management
    [Header("UI References")]
    public Text levelText; // Teks level
    public Text timerText; // Teks waktu
    public Image[] hearts; // Gambar nyawa
    public GameObject winPanel; // Panel kemenangan
    public Text winText; // Teks kemenangan
    public Button playAgainButton; // Tombol main ulang
    public Button exitButton; // Tombol keluar
    public GameObject difficultyPanel; // Panel kesulitan
    public Button easyButton; // Tombol Easy
    public Button mediumButton; // Tombol Medium
    public Button hardButton; // Tombol Hard

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Inisialisasi UI saat scene Game3Scene dimuat
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game3Scene")
        {
            InitializeUI();
            ShowDifficultyPanel();
        }
        else
        {
            // Bersihkan referensi UI
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
    #endregion

    #region Audio
    [Header("Audio Settings")]
    public AudioClip levelUpSound; // Suara naik level
    public AudioSource audioSource; // Sumber audio

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (levelUpSound != null)
        {
            audioSource.clip = levelUpSound;
        }
    }
    #endregion

    #region Game Logic
    // Mengurangi waktu dan memeriksa game over
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

    // Mengurangi nyawa
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

    // Menangani game over
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

    // Menangani kemenangan (level 5)
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
    #endregion

    #region Game Management
    // Main ulang
    void PlayAgain()
    {
        ResetGame();
        SceneManager.LoadScene("Game3Scene");
    }

    // Keluar ke GameSelection
    void ExitGame()
    {
        ResetGame();
        SceneManager.LoadScene("GameSelection");
    }

    // Tambah skor
    public void AddScore(int points)
    {
        score += points;
        if (score < 0) score = 0;
        CheckLevelUp(score);
    }

    // Reset skor
    public void ResetScore()
    {
        score = 0;
    }

    public int GetScore()
    {
        return score;
    }

    // Reset data permainan
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
            ShowDifficultyPanel();
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    // Keluar dan bersihkan data
    public void QuitGame()
    {
        StartCoroutine(QuitAndResetState());
    }

    private IEnumerator QuitAndResetState()
    {
        SceneManager.LoadScene("GameSelection");
        yield return new WaitForSeconds(0.1f);

        // Reset data
        score = 0;
        currentLevel = 1;
        lives = 3;
        timeRemaining = 0f;
        isGameActive = false;
        listenersAdded = false;
        selectedDifficulty = "Medium";

        // Bersihkan UI
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
        playAgainButton = null;
        exitButton = null;

        // Hentikan audio
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        Destroy(gameObject);
    }
    #endregion

    #region UI Initialization
    // Inisialisasi UI
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
    #endregion

    #region Difficulty Management
    // Tampilkan panel kesulitan
    private void ShowDifficultyPanel()
    {
        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(true);
            isGameActive = false;
        }
        else
        {
            Debug.LogWarning("DifficultyPanel tidak ditemukan, memulai dengan kesulitan default (Medium).");
            SelectDifficulty("Medium");
        }
    }

    // Atur parameter berdasarkan kesulitan
    private void SelectDifficulty(string difficulty)
    {
        selectedDifficulty = difficulty;
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

        InitializeGame();
        isGameActive = true;
    }
    #endregion

    #region Game Initialization
    // Atur spawner dan detektor
    private void InitializeGame()
    {
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
    #endregion

    #region Level Management
    // Periksa naik level
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
    #endregion

    #region UI Updates
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
    #endregion

    #region Difficulty Update
    // Perbarui parameter berdasarkan level
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
    #endregion

    #region Score Management
    // Simpan skor tertinggi
    public void SaveHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
    }
    #endregion

    #region Getters
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
    #endregion
}