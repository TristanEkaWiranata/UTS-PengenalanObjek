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
        if (LoadingSlider != null)
            LoadingSlider.value = 0f;

        StartCoroutine(AnimateLoadingText());
        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float displayProgress = 0f;

        while (operation.progress < 0.9f)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            displayProgress = Mathf.MoveTowards(displayProgress, targetProgress, Time.deltaTime * 0.3f); // Delay visual
            if (LoadingSlider != null)
                LoadingSlider.value = displayProgress;

            yield return null;
        }

        // Pastikan slider penuh sebelum aktivasi
        while (displayProgress < 1f)
        {
            displayProgress = Mathf.MoveTowards(displayProgress, 1f, Time.deltaTime * 0.3f);
            if (LoadingSlider != null)
                LoadingSlider.value = displayProgress;
            yield return null;
        }

        // Delay singkat untuk transisi yang halus
        yield return new WaitForSeconds(0.5f);
        operation.allowSceneActivation = true;
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
