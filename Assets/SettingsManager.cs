using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public Slider bgmSlider;

    private const string BGM_VOLUME_KEY = "BGM_VOLUME";

    void Start()
    {
        // Load saved volume
        float savedVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1.0f);
        bgmSlider.value = savedVolume;

        // Pastikan AudioManager tersedia
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetBGMVolume(savedVolume);
        }

        // Bersihkan listener lama
        bgmSlider.onValueChanged.RemoveAllListeners();

        // Tambahkan listener baru
        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);

        settingsPanel.SetActive(false); // optional
    }

    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void OnBgmVolumeChanged(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetBGMVolume(value);
        }

        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }
}
