using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingSliderManager : MonoBehaviour
{
    public Slider LoadingSlider;
    public Text LoadingText;
    public string sceneToLoad = "GameSceneHelloSpace"; // Ganti dengan nama scene yang ingin dimuat

    private string[] loadingDots = { "Loading.", "Loading..", "Loading..." };

    void Start()
    {
        StartCoroutine(AnimateLoadingText());
        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Jangan langsung pindah scene

        float fakeProgress = 0f;

        while (fakeProgress < 0.9f)
        {
            fakeProgress += Time.deltaTime * 0.3f; // Kecepatan animasi progress
            LoadingSlider.value = fakeProgress;
            yield return null;
        }

        // Tahan sedikit untuk dramatis
        yield return new WaitForSeconds(1f);

        // Isi sisa progress
        LoadingSlider.value = 1f;
        operation.allowSceneActivation = true;
    }

    IEnumerator AnimateLoadingText()
    {
        int dotIndex = 0;
        while (true)
        {
            LoadingText.text = loadingDots[dotIndex];
            dotIndex = (dotIndex + 1) % loadingDots.Length;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
