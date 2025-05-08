using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GerakObjekAntariksa : MonoBehaviour
{
    private float speed;                      // Kecepatan gerak ke kiri
    public string category;                   // "Celestial" atau "ManMade"
    public Sprite[] sprites;                  // Array sprite untuk pemilihan acak (opsional)
    private Vector3 screenPoint;
    private Vector3 offset;
    private float firstY;
    private bool isDragging = false;

    void Start()
    {
        // Validasi dan set tag berdasarkan kategori
        if (string.IsNullOrEmpty(category) || (category != "Celestial" && category != "ManMade"))
        {
            Debug.LogError($"Category tidak valid pada {gameObject.name}. Harus 'Celestial' atau 'ManMade'. Menggunakan default: Celestial");
            category = "Celestial";
        }
        gameObject.tag = category;

        // Validasi BoxCollider2D
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            Debug.LogError($"BoxCollider2D tidak ditemukan pada {gameObject.name}. Menambahkan BoxCollider2D.");
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        if (!collider.isTrigger)
        {
            Debug.LogWarning($"BoxCollider2D pada {gameObject.name} bukan trigger. Mengatur IsTrigger = true.");
            collider.isTrigger = true;
        }
        // Sesuaikan ukuran collider agar cukup besar
        if (collider.size.magnitude < 0.5f)
        {
            Debug.LogWarning($"BoxCollider2D pada {gameObject.name} terlalu kecil. Mengatur ukuran ke (1, 1).");
            collider.size = new Vector2(1f, 1f);
        }

        // (Opsional) Pilih sprite acak jika array sprites diisi
        if (sprites != null && sprites.Length > 0)
        {
            int index = Random.Range(0, sprites.Length);
            GetComponent<SpriteRenderer>().sprite = sprites[index];
        }

        // Simpan posisi Y awal
        firstY = transform.position.y;
        // Set kecepatan awal dari LevelManager
        speed = LevelManager.Instance.GetObjectSpeed();
        // Normalisasi ukuran sprite
        NormalizeSpriteSize normalizer = GetComponent<NormalizeSpriteSize>();
        if (normalizer != null)
        {
            normalizer.NormalizeSize();
        }

        // Verifikasi tag dan collider
        Debug.Log($"Objek {gameObject.name} memiliki tag: {gameObject.tag}, Collider: {collider.size}, IsTrigger: {collider.isTrigger}");
    }

    void Update()
    {
        // Jika tidak sedang di-drag, objek bergerak ke kiri
        if (!isDragging)
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

        // Hapus objek jika terlalu jauh ke kiri
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        Debug.Log($"Mulai drag pada {gameObject.name}");
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    void OnMouseUp()
    {
        isDragging = false;
        // Kunci posisi Y agar tetap di jalur
        transform.position = new Vector3(transform.position.x, firstY, transform.position.z);
        Debug.Log($"Akhir drag pada {gameObject.name} di posisi {transform.position}");
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}