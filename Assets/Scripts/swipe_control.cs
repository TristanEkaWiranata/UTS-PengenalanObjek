using UnityEngine;
using UnityEngine.UI;

public class swipe_control : MonoBehaviour
{
    public GameObject scrollbar;
    private float[] pos;
    private int currentIndex = 0;
    private float targetPos;

    void Start()
    {
        // Hitung posisi snap berdasarkan jumlah anak
        int count = transform.childCount;
        pos = new float[count];
        float distance = 1f / (count - 1f);
        for (int i = 0; i < count; i++)
        {
            pos[i] = distance * i;
        }

        targetPos = pos[currentIndex]; // Awal di index 0
    }

    void Update()
    {
        // Cek input panah kiri
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex = Mathf.Max(0, currentIndex - 1);
            targetPos = pos[currentIndex];
        }

        // Cek input panah kanan
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex = Mathf.Min(pos.Length - 1, currentIndex + 1);
            targetPos = pos[currentIndex];
        }

        // Smooth move ke posisi target
        scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, targetPos, 0.1f);
    }
}
