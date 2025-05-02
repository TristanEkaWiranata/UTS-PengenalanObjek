using UnityEngine;

public class SceneBGMSetter : MonoBehaviour
{
    [Header("BGM untuk Scene Ini")]
    public AudioClip sceneBGM;

    [Range(0f, 1f)]
    [Tooltip("Volume khusus untuk BGM di scene ini (0.0 - 1.0)")]
    public float bgmVolume = 1f;

    private AudioClip previousBGM; // Menyimpan BGM sebelumnya
    private bool wasMainBGMPlaying = false; // Menyimpan status apakah BGM utama sedang diputar

    void Start()
    {
        if (AudioManager.instance != null && sceneBGM != null)
        {
            // Simpan BGM sebelumnya jika ada
            previousBGM = AudioManager.instance.bgmSource.clip;

            // Cek apakah BGM utama sedang diputar
            wasMainBGMPlaying = AudioManager.instance.bgmSource.isPlaying;

            // Hentikan BGM utama jika sedang diputar
            if (wasMainBGMPlaying)
            {
                AudioManager.instance.bgmSource.Pause();
            }

            // Ganti BGM jika berbeda
            if (AudioManager.instance.bgmSource.clip != sceneBGM)
            {
                AudioManager.instance.ChangeBGM(sceneBGM);
            }

            // Atur volume
            AudioManager.instance.SetBGMVolume(bgmVolume);
        }
        else
        {
            Debug.LogWarning("AudioManager atau sceneBGM belum diset di SceneBGMSetter.");
        }
    }

    void OnDisable()
    {
        // Jika ada BGM sebelumnya, dan BGM utama diputar sebelumnya, putar kembali
        if (AudioManager.instance != null && previousBGM != null)
        {
            AudioManager.instance.ChangeBGM(previousBGM);
            if (wasMainBGMPlaying)
            {
                AudioManager.instance.bgmSource.Play();
            }
        }
    }
}
