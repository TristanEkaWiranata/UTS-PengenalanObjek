using UnityEngine;
using UnityEngine.UI;

public class PlanetManager : MonoBehaviour
{
    public Text titleText;
    public Text infoText;
    public Button nextButton;
    public Button prevButton;
    public Button soundOnButton;
    public Button soundOffButton;
    public Image planetImage;
    public Sprite[] planetSprites;

    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip[] planetSounds;

    private string[] planetNames = {
        "MERKURIUS", "VENUS", "BUMI", "MARS",
        "JUPITER", "SATURNUS", "URANUS", "NEPTUNUS"
    };

    private string[] planetInfos = {
        "Merkurius adalah planet terkecil dan terdekat dari Matahari...",
        "Venus adalah planet kedua dari Matahari...",
        "Bumi adalah planet ketiga dari Matahari...",
        "Mars adalah planet keempat dari Matahari...",
        "Jupiter adalah planet kelima dari Matahari...",
        "Saturnus adalah planet keenam dari Matahari...",
        "Uranus adalah planet ketujuh dari Matahari...",
        "Neptunus adalah planet kedelapan dari Matahari..."
    };

    private int currentPlanetIndex = 0;
    private bool isSoundOn = true;
    private Vector3 originalScale;

    void Start()
    {
        nextButton.onClick.AddListener(NextPlanet);
        prevButton.onClick.AddListener(PrevPlanet);
        soundOnButton.onClick.AddListener(EnableSound);
        soundOffButton.onClick.AddListener(DisableSound);

        originalScale = planetImage.rectTransform.localScale;
        UpdatePlanetInfo();
        UpdateSoundToggleUI();
        UpdateButtonState();
    }

    void UpdatePlanetInfo()
    {
        titleText.text = planetNames[currentPlanetIndex];
        infoText.text = planetInfos[currentPlanetIndex];

        if (planetSprites != null && currentPlanetIndex < planetSprites.Length)
        {
            planetImage.sprite = planetSprites[currentPlanetIndex];
        }

        audioSource.Stop();

        if (isSoundOn && planetSounds != null && currentPlanetIndex < planetSounds.Length)
        {
            audioSource.PlayOneShot(planetSounds[currentPlanetIndex]);
        }

        // Animasi untuk planet dan teks
        AnimatePlanetImage();
    }

    void NextPlanet()
    {
        if (currentPlanetIndex < planetNames.Length - 1)
        {
            currentPlanetIndex++;
            PlayClickSound();
            UpdatePlanetInfo();
            AnimatePlanetImage();
            UpdateButtonState();
        }
    }

    void PrevPlanet()
    {
        if (currentPlanetIndex > 0)
        {
            currentPlanetIndex--;
            PlayClickSound();
            UpdatePlanetInfo();
            AnimatePlanetImage();
            UpdateButtonState();
        }
    }

    void UpdateButtonState()
    {
        // Menonaktifkan tombol jika sudah di planet pertama atau terakhir
        nextButton.interactable = currentPlanetIndex < planetNames.Length - 1;
        prevButton.interactable = currentPlanetIndex > 0;
    }

    void PlayClickSound()
    {
        if (audioSource != null && buttonClickSound != null && isSoundOn)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    void EnableSound()
    {
        isSoundOn = true;
        UpdateSoundToggleUI();
        audioSource.PlayOneShot(planetSounds[currentPlanetIndex]);
    }

    void DisableSound()
    {
        isSoundOn = false;
        audioSource.Stop();
        UpdateSoundToggleUI();
    }

    void UpdateSoundToggleUI()
    {
        soundOnButton.gameObject.SetActive(!isSoundOn);
        soundOffButton.gameObject.SetActive(isSoundOn);
    }

    void AnimatePlanetImage()
    {
        planetImage.rectTransform.localScale = originalScale;
        StopAllCoroutines();
        StartCoroutine(ScaleBounce());
    }

    System.Collections.IEnumerator ScaleBounce()
    {
        float duration = 0.25f;
        float elapsed = 0f;
        Vector3 targetScale = originalScale * 1.1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            planetImage.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            planetImage.rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            yield return null;
        }

        planetImage.rectTransform.localScale = originalScale;
    }


    System.Collections.IEnumerator FadeInText()
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            titleText.canvasRenderer.SetAlpha(elapsed / duration);  // Gradually increase alpha
            yield return null;
        }

        titleText.canvasRenderer.SetAlpha(1);  // Ensure it is fully visible at the end
    }
}
