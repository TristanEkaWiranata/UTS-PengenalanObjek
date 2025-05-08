using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text finalScoreText; // UI untuk skor akhir
    public Text finalLevelText; // UI untuk level terakhir
    float timer = 0;

    void Start()
    {
        // Tampilkan skor dan level akhir
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + Data.score;
        }
        if (finalLevelText != null)
        {
            finalLevelText.text = "Level Reached: " + LevelManager.Instance.GetCurrentLevel();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 2)
        {
            Data.score = 0;
            SceneManager.LoadScene("Game3Scene");
        }
    }
}