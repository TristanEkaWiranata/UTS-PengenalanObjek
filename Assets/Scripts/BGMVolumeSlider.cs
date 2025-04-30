using UnityEngine;
using UnityEngine.UI;

public class BGMVolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioSource bgmSource;

    void Start()
    {
        // Inisialisasi nilai slider dari PlayerPrefs (kalau ada)
        float savedVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        // Tambahkan listener ke slider
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = value;
            PlayerPrefs.SetFloat("BGMVolume", value); // simpan volume
        }
    }
}
