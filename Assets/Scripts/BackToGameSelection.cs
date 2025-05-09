using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToGameSelection : MonoBehaviour
{
    // Nama scene tujuan ketika Escape ditekan
    public string targetScene = "GameSelection";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}
