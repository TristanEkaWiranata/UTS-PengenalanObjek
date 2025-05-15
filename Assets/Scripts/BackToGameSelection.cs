using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToGameSelection : MonoBehaviour
{
    public string targetScene = "GameSelection";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Reset dan keluar dari game yang sesuai berdasarkan scene atau GameManager yang aktif
            if (GameManager.Instance != null) 
            {
                GameManager.Instance.ResetGame();
            }
            else if (DragonBallGameManager.instance != null) 
            {
                DragonBallGameManager.instance.ExitToMenu();
            }

            // Bersihkan semua PlayerPrefs untuk memastikan tidak ada data sisa
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            // Pindah ke scene GameSelection
            SceneManager.LoadScene(targetScene);
        }
    }
}