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
        "Merkurius adalah planet terkecil dan terdekat dari Matahari. Permukaannya berbatu dan dipenuhi kawah akibat hantaman meteor, mirip seperti Bulan. Karena tidak memiliki atmosfer yang signifikan, suhu di planet ini berubah sangat ekstrem—mencapai lebih dari 400°C saat siang dan turun hingga -180°C di malam hari. Merkurius berputar sangat lambat, namun mengelilingi Matahari dengan kecepatan tertinggi di antara semua planet.",
        "Venus adalah planet kedua dari Matahari dan memiliki ukuran serta struktur yang hampir mirip dengan Bumi. Namun, atmosfernya yang sangat tebal terdiri dari karbon dioksida dan awan asam sulfat, menyebabkan efek rumah kaca ekstrem yang menjadikan Venus sebagai planet terpanas di tata surya. Permukaannya penuh dengan gunung berapi dan dataran luas, namun tidak terlihat langsung dari luar karena selimut awan yang pekat. Venus berputar sangat lambat dan berlawanan arah dari kebanyakan planet lain.",
        "Bumi adalah satu-satunya planet yang diketahui mendukung kehidupan. Dikenal sebagai “Planet Biru”, Bumi memiliki atmosfer kaya oksigen, air dalam wujud cair, dan suhu yang relatif stabil. Dengan satu bulan alami dan medan magnet pelindung, Bumi memiliki kondisi yang ideal bagi keberlangsungan kehidupan dalam berbagai bentuk, dari mikroorganisme hingga manusia. Planet ini terdiri dari daratan, lautan, dan lapisan atmosfer yang kompleks dan dinamis.",
        "Mars sering dijuluki “Planet Merah” karena permukaannya yang mengandung banyak besi oksida. Planet ini memiliki gunung tertinggi di tata surya, Olympus Mons, serta lembah besar bernama Valles Marineris. Atmosfer Mars sangat tipis dan hampir seluruhnya terdiri dari karbon dioksida, menjadikannya planet dingin dan kering. Meski demikian, terdapat es di kutub dan bukti adanya air di masa lampau, menjadikan Mars target utama eksplorasi untuk mencari kehidupan di luar Bumi.",
        "Jupiter adalah planet terbesar di tata surya dan termasuk dalam kategori raksasa gas. Planet ini tidak memiliki permukaan padat dan diselimuti oleh awan tebal yang berputar cepat. Ciri khas Jupiter adalah Bintik Merah Besar, sebuah badai raksasa yang telah berlangsung selama ratusan tahun. Dengan lebih dari 90 bulan alami—termasuk Ganymede, bulan terbesar di tata surya—Jupiter juga memiliki medan magnet yang sangat kuat dan sistem cincin tipis yang mengelilinginya.",
        "Saturnus terkenal dengan cincin indahnya yang terdiri dari partikel es dan batu yang mengorbit secara simetris. Seperti Jupiter, Saturnus adalah raksasa gas dengan atmosfer yang kaya akan hidrogen dan helium. Planet ini memiliki lebih dari 80 bulan, dan salah satu yang paling menonjol adalah Titan—bulan dengan atmosfer tebal dan danau metana cair di permukaannya. Saturnus berputar dengan cepat dan memiliki warna kekuningan yang khas.",
        "Uranus adalah planet raksasa es yang memiliki warna biru pucat akibat kandungan metana di atmosfernya. Ciri unik Uranus adalah sumbu rotasinya yang hampir horizontal, sehingga planet ini tampak seperti berputar sambil berbaring. Suhu di Uranus sangat rendah, bahkan termasuk yang terdingin di tata surya. Uranus memiliki lebih dari 25 bulan dan sistem cincin redup yang sulit diamati dari Bumi.",
        "Sebagai planet terjauh dari Matahari, Neptunus adalah dunia biru gelap yang diselimuti oleh badai besar dan angin tercepat yang pernah tercatat di tata surya—bahkan melampaui kecepatan suara. Seperti Uranus, Neptunus adalah raksasa es, namun memiliki lebih banyak aktivitas atmosfer. Triton, bulan terbesarnya, mengorbit secara retrograde dan kemungkinan merupakan objek sabuk Kuiper yang tertangkap gravitasi Neptunus."
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
