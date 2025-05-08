using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Text finalScoreText; // UI untuk skor akhir
    public Text finalLevelText; // UI untuk level terakhir
    public Button playAgainButton; // Tombol main lagi
    public Button quitButton; // Tombol keluar

    void Start()
    {
        // Log semua Text dan Button untuk debug
        Debug.Log("GameOverUI Start: Mencari UI dan tombol...");
        foreach (var text in FindObjectsOfType<Text>(true))
        {
            Debug.Log($"Text ditemukan: {text.gameObject.name} (Active: {text.gameObject.activeInHierarchy})");
        }
        foreach (var button in FindObjectsOfType<Button>(true))
        {
            Debug.Log($"Button ditemukan: {button.gameObject.name} (Active: {button.gameObject.activeInHierarchy})");
        }

        // Inisialisasi finalScoreText
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + GameManager.Instance.GetScore() + "\nHigh Score: " + PlayerPrefs.GetInt("HighScore", 0);
            Debug.Log($"finalScoreText diatur: Final Score={GameManager.Instance.GetScore()}");
        }
        else
        {
            Debug.LogError("finalScoreText tidak diatur. Mencoba mencari...");
            finalScoreText = GameObject.Find("FinalScoreText")?.GetComponent<Text>() ??
                             GameObject.Find("FinalScore")?.GetComponent<Text>() ??
                             FindTextWithNameContaining("score");
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + GameManager.Instance.GetScore() + "\nHigh Score: " + PlayerPrefs.GetInt("HighScore", 0);
                Debug.Log("finalScoreText ditemukan secara dinamis.");
            }
            else
            {
                Debug.LogError("finalScoreText tidak ditemukan. Pastikan ada Text bernama 'FinalScoreText' atau mengandung 'score'.");
            }
        }

        // Inisialisasi finalLevelText
        if (finalLevelText != null)
        {
            finalLevelText.text = "Level Reached: " + GameManager.Instance.GetCurrentLevel();
            Debug.Log($"finalLevelText diatur: Level={GameManager.Instance.GetCurrentLevel()}");
        }
        else
        {
            Debug.LogError("finalLevelText tidak diatur. Mencoba mencari...");
            finalLevelText = GameObject.Find("FinalLevelText")?.GetComponent<Text>() ??
                             GameObject.Find("FinalLevel")?.GetComponent<Text>() ??
                             FindTextWithNameContaining("level");
            if (finalLevelText != null)
            {
                finalLevelText.text = "Level Reached: " + GameManager.Instance.GetCurrentLevel();
                Debug.Log("finalLevelText ditemukan secara dinamis.");
            }
            else
            {
                Debug.LogError("finalLevelText tidak ditemukan. Pastikan ada Text bernama 'FinalLevelText' atau mengandung 'level'.");
            }
        }

        // Inisialisasi playAgainButton
        if (playAgainButton != null)
        {
            playAgainButton.onClick.RemoveAllListeners();
            playAgainButton.onClick.AddListener(PlayAgain);
            Debug.Log($"playAgainButton diatur: {playAgainButton.name}");
        }
        else
        {
            Debug.LogError("playAgainButton tidak diatur. Mencoba mencari...");
            playAgainButton = GameObject.Find("PlayAgainButton")?.GetComponent<Button>() ??
                              GameObject.Find("PlayAgain")?.GetComponent<Button>() ??
                              GameObject.Find("RetryButton")?.GetComponent<Button>();
            if (playAgainButton != null)
            {
                playAgainButton.onClick.RemoveAllListeners();
                playAgainButton.onClick.AddListener(PlayAgain);
                Debug.Log("playAgainButton ditemukan secara dinamis.");
            }
            else
            {
                Debug.LogError("playAgainButton tidak ditemukan. Pastikan ada Button bernama 'PlayAgainButton', 'PlayAgain', atau 'RetryButton'.");
            }
        }

        // Inisialisasi quitButton
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
            Debug.Log($"quitButton diatur: {quitButton.name}");
        }
        else
        {
            Debug.LogError("quitButton tidak diatur. Mencoba mencari...");
            quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>() ??
                         GameObject.Find("Quit")?.GetComponent<Button>() ??
                         GameObject.Find("ExitButton")?.GetComponent<Button>();
            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(QuitGame);
                Debug.Log("quitButton ditemukan secara dinamis.");
            }
            else
            {
                Debug.LogError("quitButton tidak ditemukan. Pastikan ada Button bernama 'QuitButton', 'Quit', atau 'ExitButton'.");
            }
        }

        // Nonaktifkan panel saat Start
        gameObject.SetActive(false);
    }

    private Text FindTextWithNameContaining(string substring)
    {
        foreach (var text in FindObjectsOfType<Text>(true))
        {
            if (text.name.ToLower().Contains(substring.ToLower()))
            {
                return text;
            }
        }
        return null;
    }

    void PlayAgain()
    {
        Debug.Log("Play Again ditekan: Mereset game.");
        GameManager.Instance.ResetGame();
    }

    void QuitGame()
    {
        Debug.Log("Quit ditekan: Menutup aplikasi.");
        Application.Quit();
    }
}