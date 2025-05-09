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
        }
        else
        {
            Debug.LogError($"TextScore tidak diatur di {gameObject.name}. Pastikan Text Score diassign di Inspector.");
        }

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
        }
        else
        {
            GameManager.Instance.AddScore(-15);
            objectsSorted++;

            if (textScore != null)
                textScore.text = GameManager.Instance.GetScore().ToString();

            mediaPlayerSalah.Play();
        }

        Destroy(collision.gameObject);

        GameManager.Instance.CheckLevelUp(GameManager.Instance.GetScore());

        if (objectsSorted >= GameManager.Instance.GetObjectsToSort())
        {
            GameManager.Instance.SaveHighScore();
            GameManager.Instance.TriggerGameOver();
        }
    }

    public void ResetDetector()
    {
        objectsSorted = 0;

        if (textScore != null)
        {
            textScore.text = GameManager.Instance.GetScore().ToString();
        }
        else
        {
            Debug.LogWarning($"Detektor {gameObject.name}: textScore null saat reset.");
        }
    }
}