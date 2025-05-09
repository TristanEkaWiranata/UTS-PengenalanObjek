using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOver : MonoBehaviour
{
    public Text finalScoreText;
    public Text finalLevelText;
    public Button playAgainButton;
    // public Button quitButton;

    void Start()
    {
        EnsureEventSystem();

        SetupText(ref finalScoreText, "score", () =>
        {
            if (GameManager.Instance != null)
            {
                int score = GameManager.Instance.GetScore();
                int highScore = PlayerPrefs.GetInt("HighScore", 0);
                return $"Final Score: {score}\nHigh Score: {highScore}";
            }
            return "Final Score: N/A";
        });

        SetupText(ref finalLevelText, "level", () =>
        {
            if (GameManager.Instance != null)
                return "Level Reached: " + GameManager.Instance.GetCurrentLevel();
            return "Level Reached: N/A";
        });

        SetupButton(ref playAgainButton, "PlayAgain", PlayAgain);
        // SetupButton(ref quitButton, "Quit", QuitGame);
    }

    void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }

    void SetupText(ref Text textComponent, string keyword, System.Func<string> getText)
    {
        if (textComponent == null)
        {
            textComponent = FindTextWithNameContaining(keyword);
            if (textComponent == null)
            {
                Debug.LogError($"Text dengan keyword '{keyword}' tidak ditemukan.");
                return;
            }
        }

        textComponent.text = getText();
    }

    void SetupButton(ref Button buttonComponent, string keyword, UnityEngine.Events.UnityAction action)
    {
        if (buttonComponent == null)
        {
            buttonComponent = FindButtonWithNameContaining(keyword);
            if (buttonComponent == null)
            {
                Debug.LogError($"Button dengan keyword '{keyword}' tidak ditemukan.");
                return;
            }
        }

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(action);
        Debug.Log($"{buttonComponent.name} siap digunakan.");
    }

    Text FindTextWithNameContaining(string keyword)
    {
        foreach (var text in FindObjectsOfType<Text>(true))
        {
            if (text.name.ToLower().Contains(keyword.ToLower()))
                return text;
        }
        return null;
    }

    Button FindButtonWithNameContaining(string keyword)
    {
        foreach (var button in FindObjectsOfType<Button>(true))
        {
            if (button.name.ToLower().Contains(keyword.ToLower()))
                return button;
        }
        return null;
    }

    void PlayAgain()
    {
        Debug.Log("Button Play Again ditekan.");
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGame();

        SceneManager.LoadScene("Game3Scene");
    }

    // void QuitGame()
    // {
    //     Debug.Log("Quit ditekan. Kembali ke GameSelection.");
    //     SceneManager.LoadScene("GameSelection");
    // }
}
