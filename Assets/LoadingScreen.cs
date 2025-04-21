using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Text loadingText;
    private string sceneToLoad;

    void Start()
    {
        sceneToLoad = PlayerPrefs.GetString("SceneToLoad");
        StartCoroutine(AnimateLoadingText());
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f); // delay opsional

        operation.allowSceneActivation = true;
    }

    IEnumerator AnimateLoadingText()
    {
        string baseText = "Loading";
        while (true)
        {
            loadingText.text = baseText + ".";
            yield return new WaitForSeconds(0.5f);
            loadingText.text = baseText + "..";
            yield return new WaitForSeconds(0.5f);
            loadingText.text = baseText + "...";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
