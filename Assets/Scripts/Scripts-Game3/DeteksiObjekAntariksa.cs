using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeteksiObjekAntariksa : MonoBehaviour
{
    public string targetTag;
    public AudioClip audioBenar;
    public AudioClip audioSalah;
    private AudioSource mediaPlayerBenar;
    private AudioSource mediaPlayerSalah;
    public Text textScore;
    private int objectsSorted = 0;

    void Start()
    {
        mediaPlayerBenar = gameObject.AddComponent<AudioSource>();
        mediaPlayerBenar.clip = audioBenar;

        mediaPlayerSalah = gameObject.AddComponent<AudioSource>();
        mediaPlayerSalah.clip = audioSalah;

        if (textScore != null)
        {
            textScore.text = GameManager.Instance.GetScore().ToString();
            Debug.Log($"DeteksiObjekAntariksa ({gameObject.name}): textScore diatur.");
        }
        else
        {
            Debug.LogError($"TextScore tidak diatur di {gameObject.name}. Pastikan Text Score diassign di Inspector.");
        }

        Debug.Log($"DeteksiObjekAntariksa ({gameObject.name}) diinisialisasi: targetTag={targetTag}");
        ResetDetector();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            int finalScore = 25;
            GameManager.Instance.AddScore(finalScore);
            objectsSorted++;

            if (textScore != null)
                textScore.text = GameManager.Instance.GetScore().ToString();

            mediaPlayerBenar.Play();
            Debug.Log($"Objek {collision.gameObject.name} benar di {gameObject.name}. Skor: +{finalScore}, objectsSorted={objectsSorted}");
        }
        else
        {
            GameManager.Instance.AddScore(-15);
            objectsSorted++;

            if (textScore != null)
                textScore.text = GameManager.Instance.GetScore().ToString();

            mediaPlayerSalah.Play();
            Debug.Log($"Objek {collision.gameObject.name} salah di {gameObject.name}. Skor: -15, objectsSorted={objectsSorted}");
        }

        Destroy(collision.gameObject);

        GameManager.Instance.CheckLevelUp(GameManager.Instance.GetScore());

        if (objectsSorted >= GameManager.Instance.GetObjectsToSort())
        {
            GameManager.Instance.SaveHighScore();
            GameManager.Instance.TriggerGameOver();
            Debug.Log($"Game Over: Semua objek ({objectsSorted}/{GameManager.Instance.GetObjectsToSort()}) disortir.");
        }
    }

    public void ResetDetector()
    {
        objectsSorted = 0;

        if (textScore != null)
        {
            textScore.text = GameManager.Instance.GetScore().ToString();
            Debug.Log($"Detektor {gameObject.name} direset: objectsSorted={objectsSorted}, score={GameManager.Instance.GetScore()}");
        }
        else
        {
            Debug.LogWarning($"Detektor {gameObject.name}: textScore null saat reset.");
        }
    }
}