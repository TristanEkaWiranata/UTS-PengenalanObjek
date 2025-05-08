using UnityEngine;

public class NormalizeSpriteSize : MonoBehaviour
{
    [SerializeField] private float referencePixelSize = 64f; // Ukuran referensi (misalnya, 64 piksel)
    [SerializeField] private float targetWorldSize = 1f; // Ukuran di dunia Unity (1 unit)

    void Start()
    {
        NormalizeSize();
    }

    public void NormalizeSize() // Changed from private/implicit to public
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            // Dapatkan ukuran piksel sprite
            float spritePixelWidth = spriteRenderer.sprite.rect.width;
            // Hitung faktor skala berdasarkan ukuran referensi
            float scaleFactor = referencePixelSize / spritePixelWidth;
            // Terapkan skala agar ukuran di dunia sama
            transform.localScale = new Vector3(scaleFactor * targetWorldSize, scaleFactor * targetWorldSize, 1f);
        }
    }
}