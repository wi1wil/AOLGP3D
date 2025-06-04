using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public int hp = 100;
    public GameObject gameOverPanel;
    public Button restartButton;
    public TMP_Text highScoreText;

    private int highScore = 0;

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        Time.timeScale = 1f;

        // Load high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreUI();
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Debug.Log("Player has died.");
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
            Time.timeScale = 0f;

            int currentScore = 0;
            var scoring = FindObjectOfType<ScoringScript>();
            if (scoring != null)
                currentScore = scoring.score;
            if (currentScore > highScore)
            {
                highScore = currentScore;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
            }
            UpdateHighScoreUI();
        }
        else
        {
            Debug.Log("Player took damage. Remaining HP: " + hp);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;
    }
}
