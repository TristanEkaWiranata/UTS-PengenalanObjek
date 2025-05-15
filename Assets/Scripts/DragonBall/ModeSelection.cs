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

        if (pvpButton != null) {
            pvpButton.onClick.AddListener(PlayPVP);
        }
        else
            Debug.LogError("PVPButton tidak ditemukan di scene!");

        if (pveButton != null) {
            pveButton.onClick.AddListener(PlayPVE);
        }
        else
            Debug.LogError("PVEButton tidak ditemukan di scene!");
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
}
