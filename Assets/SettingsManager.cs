using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public Slider bgmSlider;

    [Header("Audio")]
    public AudioSource bgmAudioSource;

    private const string BGM_VOLUME_KEY = "BGM_VOLUME";

    void Start()
    {
        // Load saved volume or default to 1.0
        float savedVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1.0f);
        bgmSlider.value = savedVolume;
        bgmAudioSource.volume = savedVolume;

        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);

        // Optional: Panel mulai disembunyikan
        settingsPanel.SetActive(false);
    }

    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void OnBgmVolumeChanged(float value)
    {
        bgmAudioSource.volume = value;
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }
}
