using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverSceneManager : MonoBehaviour
{
    public Text resultText;
    public Button restartButton;
    public Button exitMenuButton;

    void Start()
    {
        if (resultText != null)
            resultText.text = DragonBallGameManager.lastGameOverMessage;

        restartButton.onClick.AddListener(() =>
        {
            Debug.Log("Restart button clicked!");
            DragonBallGameManager.instance.RestartGame();
        });

        exitMenuButton.onClick.AddListener(() =>
        {
            Debug.Log("Quit button clicked!");
            DragonBallGameManager.instance.ExitToMenu();

        });
    }
}
