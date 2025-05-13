using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button exitToDesktopButton;
    public ExitConfirmation exitConfirmation;

    void Start()
    {
        if (exitToDesktopButton != null)
        {
            exitToDesktopButton.onClick.RemoveAllListeners();
            exitToDesktopButton.onClick.AddListener(OnExitToDesktopClicked);
        }
    }

    private void OnExitToDesktopClicked()
    {
        if (exitConfirmation != null)
        {
            exitConfirmation.ShowConfirmation(QuitGame);
        }
        else
        {
            QuitGame();
        }
    }

    private void QuitGame()
    {
        Debug.Log("Keluar dari game..."); // Saat di editor
        Application.Quit(); // Akan berfungsi saat build
    }
}
