using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 targetScale = new Vector3(1.1f, 1.1f, 1f); // Skala saat hover
    public float duration = 0.1f;

    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScaling(targetScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScaling(originalScale);
    }

    private void StartScaling(Vector3 toScale)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleTo(toScale));
    }

    private System.Collections.IEnumerator ScaleTo(Vector3 target)
    {
        Vector3 start = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(start, target, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = target;
    }
}
