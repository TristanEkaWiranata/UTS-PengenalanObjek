using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;
    public Toggle musicToggle;
    public Slider volumeSlider;
    public Text statusText;

    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);

        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);

        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
    }

    private void OnPlayButtonClicked()
    {
        AudioManager.instance?.PlaySFX();
        statusText.text = "Game Started!";
    }

    private void OnQuitButtonClicked()
    {
        AudioManager.instance?.PlaySFX();
        statusText.text = "Exiting Game...";
        Application.Quit();
    }

    private void OnMusicToggleChanged(bool isOn)
    {
        AudioManager.instance?.SetBGMMute(!isOn);
        AudioManager.instance?.PlaySFX();
    }

    private void OnVolumeSliderChanged(float value)
    {
        AudioManager.instance?.SetBGMVolume(value);
        AudioManager.instance?.PlaySFX();
    }
}
