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
                int score = PlayerPrefs.GetInt("LastScore", 0);
                int highScore = PlayerPrefs.GetInt("HighScore", 0);
                return $"Final Score: {score}\nHigh Score: {highScore}";
            });

            SetupText(ref finalLevelText, "level", () =>
            {
                int level = PlayerPrefs.GetInt("LastLevel", 1);
                return $"Level Reached: {level}";
            });

            SetupButton(ref playAgainButton, "PlayAgain", PlayAgain);
            // SetupButton(ref quitButton, "Quit", QuitGame);
        }


        void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() == null)
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
            foreach (var text in FindObjectsByType<Text>(FindObjectsSortMode.None))
            {
                if (text.name.ToLower().Contains(keyword.ToLower()))
                    return text;
            }
            return null;
        }

        Button FindButtonWithNameContaining(string keyword)
        {
            foreach (var button in FindObjectsByType<Button>(FindObjectsSortMode.None))
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

        void QuitGame()
        {
            Debug.Log("Quit ditekan. Kembali ke GameSelection.");
            if (GameManager.Instance != null)
                GameManager.Instance.ResetGame();
            SceneManager.LoadScene("GameSelection");
        }
    }
