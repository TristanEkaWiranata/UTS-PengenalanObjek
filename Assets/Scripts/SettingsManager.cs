using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public Slider bgmSlider;
    public Button closeButton; // Tambahkan referensi tombol close

    private const string BGM_VOLUME_KEY = "BGM_VOLUME";

    void Start()
    {
        // Load saved volume
        float savedVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1.0f);
        bgmSlider.value = savedVolume;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetBGMVolume(savedVolume);
        }

        bgmSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);

        // Tambahkan listener untuk tombol close
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseSettingsPanel);
        }

        settingsPanel.SetActive(false);
    }

    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
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
