using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    [Header("Optional: Ganti BGM Saat Pindah Scene")]
    public AudioClip newBGMClip;

    public void LoadScene(string sceneName)
    {
        // Ganti BGM jika AudioManager aktif dan ada clip baru
        if (AudioManager.instance != null && newBGMClip != null)
        {
            AudioManager.instance.ChangeBGM(newBGMClip);
        }

        // Pindah ke scene tujuan
        SceneManager.LoadScene(sceneName);
    }
}
