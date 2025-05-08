using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Text levelText;
    public AudioClip levelUpSound;
    private AudioSource audioSource;

    private int currentLevel = 1;
    private readonly int[] scoreThresholds = { 200, 400 }; // Skor untuk naik level
    private readonly float[] spawnIntervals = { 1.0f, 0.8f, 0.67f }; // Interval spawn per level
    private readonly float[] objectSpeeds = { 4.0f, 5.0f, 6.0f }; // Kecepatan objek per level
    private readonly int[] objectsToSort = { 20, 25, 30 }; // Jumlah objek per level
    private int objectsSorted = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = levelUpSound;
        UpdateLevelUI();
    }

    public void CheckLevelUp(int score)
    {
        for (int i = 0; i < scoreThresholds.Length; i++)
        {
            if (score >= scoreThresholds[i] && currentLevel < i + 2)
            {
                currentLevel = i + 2;
                audioSource.Play();
                UpdateLevelUI();
                UpdateGameDifficulty();
                ResetObjectsSorted();
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

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void IncrementObjectsSorted()
    {
        objectsSorted++;
        if (objectsSorted >= GetObjectsToSort())
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    public void ResetObjectsSorted()
    {
        objectsSorted = 0;
    }
}