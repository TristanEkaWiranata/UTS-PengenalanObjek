using UnityEngine;
using System.Collections;

public class GameCreditController : MonoBehaviour
{
    public GameObject gameCreditPanel;
    public GameObject BtnKeluar;
    public float animationDuration = 0.5f;

    private RectTransform creditRect;
    private Vector2 hiddenPosition;
    private Vector2 visiblePosition;

    private float lastClickTime;
    private float doubleClickThreshold = 0.3f;

    void Start()
    {
        creditRect = gameCreditPanel.GetComponent<RectTransform>();

        // Posisi akhir saat muncul (tengah layar)
        visiblePosition = creditRect.anchoredPosition;

        // Posisi tersembunyi di bawah layar
        hiddenPosition = new Vector2(visiblePosition.x, -Screen.height);

        // Set posisi awal ke tersembunyi dan nonaktifkan panel
        creditRect.anchoredPosition = hiddenPosition;
        gameCreditPanel.SetActive(false);
    }

    void Update()
    {
        if (gameCreditPanel.activeSelf)
        {
            // Deteksi double click kiri
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time - lastClickTime < doubleClickThreshold)
                {
                    HideCredit(); // Double click detected
                }

                lastClickTime = Time.time;
            }
        }
    }

    public void ShowCredit()
    {
        gameCreditPanel.SetActive(true);
        BtnKeluar.SetActive(false); // Sembunyikan tombol keluar

        StopAllCoroutines();
        StartCoroutine(Slide(creditRect, hiddenPosition, visiblePosition, animationDuration));
    }

    public void HideCredit()
    {
        StopAllCoroutines();
        StartCoroutine(SlideAndDeactivate(creditRect, visiblePosition, hiddenPosition, animationDuration));
        BtnKeluar.SetActive(true); // Tampilkan tombol keluar kembali
    }

    private IEnumerator Slide(RectTransform target, Vector2 from, Vector2 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            target.anchoredPosition = Vector2.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.anchoredPosition = to;
    }

    private IEnumerator SlideAndDeactivate(RectTransform target, Vector2 from, Vector2 to, float duration)
    {
        yield return Slide(target, from, to, duration);
        gameCreditPanel.SetActive(false);
    }
}
