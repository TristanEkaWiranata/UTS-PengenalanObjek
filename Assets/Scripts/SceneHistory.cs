using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneHistory
{
    public static string previousScene = null;

    public static void LoadSceneWithHistory(string newSceneName)
    {
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(newSceneName);
    }

    public static void LoadPreviousScene()
    {
        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("Tidak ada scene sebelumnya yang disimpan.");
        }
    }
}
