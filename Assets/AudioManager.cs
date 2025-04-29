using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource sfxSource;
    public AudioSource bgmSource;

    public AudioClip buttonClickSFX;
    public AudioClip bgmClip;

    private void Awake()
    {
        // Bikin Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // jangan dihancurkan saat pindah scene
        }
        // else
        // {
        //     Destroy(gameObject);
        // }
    }

    private void Start()
    {
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    // Function untuk play sfx
    public void PlaySFX()
    {
        if (sfxSource != null && buttonClickSFX != null)
        {
            sfxSource.PlayOneShot(buttonClickSFX);
        }
    }

    // Function untuk atur volume BGM
    public void SetBGMVolume(float value)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = value;
        }
    }

    // Function untuk mute/unmute BGM
    public void SetBGMMute(bool mute)
    {
        if (bgmSource != null)
        {
            bgmSource.mute = mute;
        }
    }
}
