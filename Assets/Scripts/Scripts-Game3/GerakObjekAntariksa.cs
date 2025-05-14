using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GerakObjekAntariksa : MonoBehaviour
{
    private float speed;
    public string category; // "Celestial" atau "ManMade"
    public Sprite[] sprites; // Array sprite untuk pemilihan acak
    private Vector3 screenPoint;
    private Vector3 offset;
    private float firstY;
    private bool isDragging = false;
    private bool isStopped = false;

    void Start()
    {
        if (string.IsNullOrEmpty(category) || (category != "Celestial" && category != "ManMade"))
        {
            Debug.LogError($"Category tidak valid pada {gameObject.name}. Harus 'Celestial' atau 'ManMade'. Menggunakan default: Celestial");
            category = "Celestial";
        }
        gameObject.tag = category;

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
        if (collider.size.magnitude < 0.5f)
        {
            Debug.LogWarning($"BoxCollider2D pada {gameObject.name} terlalu kecil. Mengatur ukuran ke (1, 1).");
            collider.size = new Vector2(1f, 1f);
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"SpriteRenderer tidak ditemukan pada {gameObject.name}.");
        }
        else if (sprites != null && sprites.Length > 0)
        {
            int index = Random.Range(0, sprites.Length);
            spriteRenderer.sprite = sprites[index];
        }
        else
        {
            Debug.LogWarning($"Array sprites kosong atau null pada {gameObject.name}. Menggunakan sprite default.");
        }

        if (gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
        {
            Debug.LogWarning($"Objek {gameObject.name} ada di layer Ignore Raycast. Mengubah ke Default.");
            gameObject.layer = LayerMask.NameToLayer("Default");
        }

        firstY = transform.position.y;
        speed = GameManager.Instance.GetObjectSpeed();
        NormalizeSpriteSize normalizer = GetComponent<NormalizeSpriteSize>();
        if (normalizer != null)
        {
            normalizer.NormalizeSize();
        }

    }

    void Update()
    {
        if (!isDragging)
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

        // if (!isStopped)
        // {
        //     transform.Translate(Vector3.down * speed * Time.deltaTime);
        // }

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
        transform.position = new Vector3(transform.position.x, firstY, transform.position.z);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void StopMovement()
    {
        isStopped = true;
    }
}