using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonClickSFX : MonoBehaviour, IPointerEnterHandler
{
    [Header("Audio Clips")]
    public AudioClip clickSound;
    public AudioClip hoverSound;

    private AudioSource audioSource;

    void Start()
    {
        // Tambahkan AudioSource jika belum ada
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Tambahkan listener untuk klik
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            if (clickSound != null)
            {
                audioSource.PlayOneShot(clickSound);
            }
            else
            {
                Debug.LogWarning("Click Sound belum disetel.");
            }
        });
    }

    // Ini dipanggil saat pointer masuk ke tombol
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
        else
        {
            Debug.LogWarning("Hover Sound belum disetel.");
        }
    }
}
