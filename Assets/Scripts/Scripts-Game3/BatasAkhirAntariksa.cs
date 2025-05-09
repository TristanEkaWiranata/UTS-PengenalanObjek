using UnityEngine;

public class BatasAkhirAntariksa : MonoBehaviour
{
    [SerializeField] private AudioClip hitSound; // Suara saat menyentuh batas
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Celestial") || collision.CompareTag("ManMade"))
        {
            Debug.Log($"Objek dengan tag 'ObjekAntariksa' terdeteksi: {collision.gameObject.name}");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }
            else
            {
                Debug.LogWarning("GameManager.Instance tidak ditemukan.");
            }

            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
            else
            {
                Debug.LogWarning("AudioSource atau hitSound tidak diatur.");
            }

            Destroy(collision.gameObject);
        }
        else
        {
            Debug.LogWarning($"Objek tidak memiliki tag 'ObjekAntariksa': Tag={collision.tag}");
        }
    }
}