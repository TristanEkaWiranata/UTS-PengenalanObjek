using UnityEngine;
using UnityEngine.UI;

public class PlanetManager : MonoBehaviour
{
    public Text titleText;         // Referensi ke UI Text untuk nama planet
    public Text infoText;          // Referensi ke UI Text untuk informasi planet
    public Button nextButton;      // Tombol Next
    public Button prevButton;      // Tombol Previous
    public Image planetImage;      // UI Image untuk menampilkan sprite planet
    public Sprite[] planetSprites; // Array Sprite planet

    private string[] planetNames = {
        "MERKURIUS", "VENUS", "BUMI", "MARS",
        "JUPITER", "SATURNUS", "URANUS", "NEPTUNUS"
    };

    private string[] planetInfos = {
        "Merkurius adalah planet terkecil dan terdekat dari Matahari. Periode rotasinya sangat lambat dan dipenuhi kawah akibat hantaman meteor, mirip seperti Bulan. Karena tidal locked, permukaannya sangat ekstrem—mencapai lebih dari 400°C saat siang dan -180°C di malam hari. Merkurius berputar sangat lambat, namun mengelilingi Matahari dengan orbit tercepat di antara semua planet.",
        "Venus adalah planet kedua dari Matahari. Dikenal sebagai planet paling panas karena efek rumah kaca ekstrem, suhunya mencapai 470°C. Atmosfernya tebal, penuh dengan karbon dioksida dan awan asam sulfat. Venus berputar sangat lambat dan berlawanan arah dengan planet lain (retrograde).",
        "Bumi adalah planet ketiga dari Matahari. Planet ini unik karena memiliki air cair, kehidupan, dan atmosfer yang kaya oksigen. Suhu rata-ratanya sekitar 15°C, dengan iklim yang beragam. Bumi memiliki satu satelit alami, yaitu Bulan.",
        "Mars adalah planet keempat dari Matahari. Dikenal sebagai Planet Merah karena warna permukaannya yang kemerahan akibat oksida besi. Mars memiliki gunung berapi terbesar di tata surya, Olympus Mons, dan suhu rata-rata -63°C.",
        "Jupiter adalah planet kelima dari Matahari dan terbesar di tata surya. Planet ini merupakan raksasa gas dengan banyak satelit, termasuk Ganymede yang lebih besar dari Merkurius. Jupiter memiliki badai besar bernama Bintik Merah Besar.",
        "Saturnus adalah planet keenam dari Matahari, terkenal dengan sistem cincinnya yang indah. Planet ini juga raksasa gas, dengan banyak satelit seperti Titan yang memiliki atmosfer lebih tebal dari Bumi.",
        "Uranus adalah planet ketujuh dari Matahari. Planet ini unik karena sumbu rotasinya sangat miring, hampir sejajar dengan orbitnya. Uranus memiliki warna biru kehijauan karena kandungan metana di atmosfernya.",
        "Neptunus adalah planet kedelapan dari Matahari. Planet ini memiliki angin tercepat di tata surya, mencapai 2.400 km/jam. Neptunus berwarna biru cerah karena metana, dan memiliki satelit besar bernama Triton."
    };

    private int currentPlanetIndex = 0;

    void Start()
    {
        nextButton.onClick.AddListener(NextPlanet);
        prevButton.onClick.AddListener(PrevPlanet);

        UpdatePlanetInfo();
    }

    void NextPlanet()
    {
        currentPlanetIndex = (currentPlanetIndex + 1) % planetNames.Length;
        UpdatePlanetInfo();
    }

    void PrevPlanet()
    {
        currentPlanetIndex = (currentPlanetIndex - 1 + planetNames.Length) % planetNames.Length;
        UpdatePlanetInfo();
    }

    void UpdatePlanetInfo()
    {
        titleText.text = planetNames[currentPlanetIndex];
        infoText.text = planetInfos[currentPlanetIndex];

        // Update gambar planet
        if (planetSprites != null && currentPlanetIndex < planetSprites.Length)
        {
            planetImage.sprite = planetSprites[currentPlanetIndex];
        }
    }
}
