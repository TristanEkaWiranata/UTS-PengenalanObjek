using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeteksiObjekAntariksa : MonoBehaviour
{
    public string targetTag; // "Celestial" atau "ManMade"
    public AudioClip audioBenar;
    public AudioClip audioSalah;
    private AudioSource mediaPlayerBenar;
    private AudioSource mediaPlayerSalah;
    public Text textScore;
    private int objectsSorted = 0; // Menghitung objek yang sudah disortir di level ini

    void Start()
    {
        mediaPlayerBenar = gameObject.AddComponent<AudioSource>();
        mediaPlayerBenar.clip = audioBenar;

        mediaPlayerSalah = gameObject.AddComponent<AudioSource>();
        mediaPlayerSalah.clip = audioSalah;

        // Inisialisasi skor
        textScore.text = Data.score.ToString();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals(targetTag))
        {
            Data.score += 25;
            objectsSorted++;
            textScore.text = Data.score.ToString();
            Destroy(collision.gameObject);
            mediaPlayerBenar.Play();
        }
        else
        {
            Data.score -= 5;
            objectsSorted++;
            textScore.text = Data.score.ToString();
            Destroy(collision.gameObject);
            mediaPlayerSalah.Play();
        }

        // Periksa apakah naik level
        LevelManager.Instance.CheckLevelUp(Data.score);

        // Cek jika jumlah objek di level ini selesai
        if (objectsSorted >= LevelManager.Instance.GetObjectsToSort())
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}