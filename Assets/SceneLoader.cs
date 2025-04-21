using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneWithLoading(string targetScene)
    {
        PlayerPrefs.SetString("SceneToLoad", targetScene);
        SceneManager.LoadScene("LoadingScreen");
    }
}
