using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GerakPindah : MonoBehaviour
{
    public float speed = 3f;                   // Kecepatan gerak ke kiri
    public Sprite[] sprites;                   // Array sprite untuk dipilih acak
    private Vector3 screenPoint;
    private Vector3 offset;
    private float firstY;
    private bool isDragging = false;

    void Start()
    {
        // Pilih sprite secara acak saat muncul
        int index = Random.Range(0, sprites.Length);
        GetComponent<SpriteRenderer>().sprite = sprites[index];

        // Simpan posisi Y awal
        firstY = transform.position.y;
    }

    void Update()
    {
        // Jika tidak sedang di-drag, objek bergerak ke kiri
        if (!isDragging)
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

        // Hapus objek jika terlalu jauh ke kiri (opsional)
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
        // Kunci posisi Y agar tetap di jalur
        transform.position = new Vector3(transform.position.x, firstY, transform.position.z);
    }
}
