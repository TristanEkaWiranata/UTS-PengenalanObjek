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
            int baseScore = 25;
            float timeBonus = GameManager.Instance.GetTimeRemaining() / GameManager.Instance.GetLevelTime();
            int finalScore = Mathf.RoundToInt(baseScore * (1 + timeBonus));
            GameManager.Instance.AddScore(finalScore);
            objectsSorted++;
            if (textScore != null)
            {
                textScore.text = GameManager.Instance.GetScore().ToString();
            }
            mediaPlayerBenar.Play();
            Debug.Log($"Objek {collision.gameObject.name} benar di {gameObject.name}. Skor: +{finalScore} (Bonus waktu: {timeBonus:F2}), objectsSorted={objectsSorted}");
        }
        else
        {
            GameManager.Instance.AddScore(-5);
            objectsSorted++;
            if (textScore != null)
            {
                textScore.text = GameManager.Instance.GetScore().ToString();
            }
            mediaPlayerSalah.Play();
            Debug.Log($"Objek {collision.gameObject.name} salah di {gameObject.name}. Skor: -5, objectsSorted={objectsSorted}");
        }

        Destroy(collision.gameObject);

        GameManager.Instance.CheckLevelUp(GameManager.Instance.GetScore());

        if (objectsSorted >= GameManager.Instance.GetObjectsToSort())
        {
            GameManager.Instance.SaveHighScore();
            SceneManager.LoadScene("GameOver");
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