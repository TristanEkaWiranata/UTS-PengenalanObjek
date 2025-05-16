using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Keluar : MonoBehaviour
{
    public string targetScene = "GameSelection";

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    // ...existing code...
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            // Cek referensi GameManager
            if (GameManager.Instance != null)
            {
                // Jika WinPanel atau DifficultyPanel aktif, kembali ke GameSelection
                if ((GameManager.Instance.winPanel != null && GameManager.Instance.winPanel.activeSelf) ||
                    (GameManager.Instance.difficultyPanel != null && GameManager.Instance.difficultyPanel.activeSelf))
                {
                    GameManager.Instance.ResetGame();
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                    SceneManager.LoadScene(targetScene);
                }
                else
                {
                    // Jika panel tidak aktif, aktifkan DifficultyPanel
                    if (GameManager.Instance.difficultyPanel != null)
                    {
                        GameManager.Instance.ResetGame();
                        GameManager.Instance.difficultyPanel.SetActive(true);
                        GameManager.Instance.isGameActive = false;
                    }
                }
            }
            else
            {
                // Jika GameManager tidak ditemukan, fallback ke GameSelection
                GameManager.Instance.ResetGame();
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                SceneManager.LoadScene(targetScene);
            }
        }
    }
}