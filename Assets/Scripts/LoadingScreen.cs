using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingSliderManager : MonoBehaviour
{
    public Slider LoadingSlider;
    public Text LoadingText;
    public string sceneToLoad = "GameSceneHelloSpace";

    private string[] loadingDots = { "Loading.", "Loading..", "Loading..." };

    void Start()
    {
        StartCoroutine(AnimateLoadingText());
        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (fakeProgress < 0.9f)
        {
            fakeProgress += Time.deltaTime * 0.3f;
            LoadingSlider.value = Mathf.Clamp01(fakeProgress);
            yield return null;
        }

        LoadingSlider.value = 0.9f;

        while (!operation.isDone)
         {
             if (operation.progress >= 0.9f)
             {
                 LoadingSlider.value = 1f;
                 operation.allowSceneActivation = true;
                 yield return null; // Tambahkan yield di sini untuk sedikit menunda sebelum pindah frame visual
             }

             yield return null;
         }
    }

    IEnumerator AnimateLoadingText()
    {
        int dotIndex = 0;
        while (true)
        {
            if (LoadingText != null)
            {
                LoadingText.text = loadingDots[dotIndex];
                dotIndex = (dotIndex + 1) % loadingDots.Length;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}