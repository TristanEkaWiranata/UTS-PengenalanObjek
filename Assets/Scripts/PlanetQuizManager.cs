using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetQuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public Sprite planetImage;
        public string[] answers;
        public int correctAnswerIndex;
    }

    public List<Question> questions;
    public Text questionText;
    public Text infoText;
    public Image planetImageDisplay;
    public Button[] answerButtons;

    public GameObject feedbackPanel;
    public Image feedbackImage;
    public Sprite correctSprite;
    public Sprite wrongSprite;

    public Text timerText;
    public Text scoreText;
    public GameObject resultPanel;
    public Text resultText;
    public Text finalScoreText;

    public AudioSource audioSource;
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    private List<Question> randomizedQuestions;
    private int currentQuestionIndex = 0;
    private int score = 0;

    public float timePerQuestion = 10f;
    private float timeLeft;
    private bool waitingForNext = false;

    void Start()
    {
        randomizedQuestions = new List<Question>(questions);
        Shuffle(randomizedQuestions);
        randomizedQuestions = randomizedQuestions.GetRange(0, Mathf.Min(5, randomizedQuestions.Count));

        feedbackPanel.SetActive(false);
        resultPanel.SetActive(false);
        infoText.text = "";
        LoadQuestion();
    }

    void Update()
    {
        if (!waitingForNext && timerText.gameObject.activeSelf)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = "⏱️ " + Mathf.CeilToInt(timeLeft) + "s";

            if (timeLeft <= 0f)
            {
                waitingForNext = true;
                ShowFeedback(false);
                HighlightCorrectAnswer();
                Invoke(nameof(NextQuestion), 2f);
            }
        }
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex >= randomizedQuestions.Count)
        {
            questionText.text = "";
            planetImageDisplay.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
            feedbackPanel.SetActive(false);
            scoreText.text = "";
            infoText.text = "";

            foreach (var btn in answerButtons)
                btn.gameObject.SetActive(false);

            resultPanel.SetActive(true);
            resultText.text = "🎉 Kuis Selesai!";
            finalScoreText.text = $"🏁 Skor Anda: {score} / {randomizedQuestions.Count * 10}";

            return;
        }


        ResetButtonColors();

        Question q = randomizedQuestions[currentQuestionIndex];
        questionText.text = q.questionText;
        infoText.text = "";
        planetImageDisplay.sprite = q.planetImage;
        planetImageDisplay.gameObject.SetActive(true);
        scoreText.text = $"Skor: {score}";
        timerText.gameObject.SetActive(true);
        timeLeft = timePerQuestion;
        waitingForNext = false;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<Text>().text = q.answers[i];
            answerButtons[i].gameObject.SetActive(true);

            int index = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => AnswerButtonClicked(index));
        }
    }

    void AnswerButtonClicked(int index)
    {
        if (waitingForNext) return;

        waitingForNext = true;
        Question q = randomizedQuestions[currentQuestionIndex];

        if (index == q.correctAnswerIndex)
        {
            score += 10;
            ShowFeedback(true);
            HighlightButton(index, Color.green);
        }
        else
        {
            ShowFeedback(false);
            HighlightButton(index, Color.red);
            HighlightCorrectAnswer();
        }

        Invoke(nameof(NextQuestion), 2f);
    }

    void ShowFeedback(bool isCorrect)
    {
        feedbackImage.sprite = isCorrect ? correctSprite : wrongSprite;
        feedbackPanel.SetActive(true);
        feedbackPanel.transform.localScale = Vector3.zero;
        StartCoroutine(AnimatePopup());

        if (audioSource != null)
        {
            audioSource.PlayOneShot(isCorrect ? correctSFX : wrongSFX);
        }
    }

    IEnumerator AnimatePopup()
    {
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float scale = Mathf.Lerp(0f, 1f, elapsed / duration);
            feedbackPanel.transform.localScale = new Vector3(scale, scale, scale);
            elapsed += Time.deltaTime;
            yield return null;
        }

        feedbackPanel.transform.localScale = Vector3.one;
    }

    void NextQuestion()
    {
        feedbackPanel.SetActive(false);
        currentQuestionIndex++;
        LoadQuestion();
    }

    void HighlightButton(int index, Color color)
    {
        var colors = answerButtons[index].colors;
        colors.normalColor = color;
        answerButtons[index].colors = colors;
    }

    void HighlightCorrectAnswer()
    {
        int correctIndex = randomizedQuestions[currentQuestionIndex].correctAnswerIndex;
        HighlightButton(correctIndex, Color.green);
    }

    void ResetButtonColors()
    {
        foreach (var btn in answerButtons)
        {
            var colors = btn.colors;
            colors.normalColor = Color.white;
            btn.colors = colors;
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
