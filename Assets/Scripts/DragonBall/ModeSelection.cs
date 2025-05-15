using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModeSelection : MonoBehaviour
{
    void Start()
    {
        // Cari tombol berdasarkan nama GameObject
        Button pvpButton = GameObject.Find("PVPButton")?.GetComponent<Button>();
        Button pveButton = GameObject.Find("PVEButton")?.GetComponent<Button>();
        Button quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();

        if (pvpButton != null) pvpButton.onClick.AddListener(PlayPVP);
        else Debug.LogError("PVPButton tidak ditemukan di scene!");

        if (pveButton != null) pveButton.onClick.AddListener(PlayPVE);
        else Debug.LogError("PVEButton tidak ditemukan di scene!");

        if (quitButton != null) quitButton.onClick.AddListener(Quit);
        else Debug.LogError("QuitButton tidak ditemukan di scene!");
    }

    void PlayPVP()
    {
        PlayerPrefs.SetString("GameMode", "PVP");
        SceneManager.LoadScene("GameSceneDragonBall");
    }

    void PlayPVE()
    {
        PlayerPrefs.SetString("GameMode", "PVE");
        SceneManager.LoadScene("GameSceneDragonBall");
    }

    void Quit()
    {
        SceneManager.LoadScene("GameSelection");
    }
}
