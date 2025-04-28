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
    public Image planetImageDisplay;
    public Button[] answerButtons;
    public Text feedbackText;

    private int currentQuestionIndex = 0;

    void Start()
    {
        LoadQuestion();
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            questionText.text = "Kuis selesai! 🎉";
            planetImageDisplay.gameObject.SetActive(false);
            foreach (var btn in answerButtons)
            {
                btn.gameObject.SetActive(false);
            }
            feedbackText.text = "";
            return;
        }

        Question q = questions[currentQuestionIndex];
        questionText.text = q.questionText;
        planetImageDisplay.sprite = q.planetImage;
        planetImageDisplay.gameObject.SetActive(true);
        feedbackText.text = "";

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<Text>().text = q.answers[i];

            int index = i; // penting untuk closure
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => AnswerButtonClicked(index));
        }
    }

    void AnswerButtonClicked(int index)
    {
        if (index == questions[currentQuestionIndex].correctAnswerIndex)
        {
            feedbackText.text = "Benar! ✅";
            currentQuestionIndex++;
            Invoke(nameof(LoadQuestion), 1.5f); // lanjut soal berikutnya
        }
        else
        {
            feedbackText.text = "Salah! ❌ Coba lagi.";
        }
    }
}
